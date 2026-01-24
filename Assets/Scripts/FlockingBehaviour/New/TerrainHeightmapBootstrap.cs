
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Terrain heightmap data for efficient sampling in jobs
public struct TerrainHeightmapData : IComponentData
{
    public float3 TerrainPosition;    // World position of terrain origin
    public float3 TerrainSize;        // Size of terrain in world units
    public int2 HeightmapResolution;  // Resolution of heightmap (width, height)
    public float MinFlightHeight;     // How high above terrain surface to fly
    public float AvoidanceStrength;   // Force multiplier
    public int TerrainID;             // Unique ID for this terrain
    public int2 GridCoords;           // Grid coordinates for spatial lookup
}

// The actual heightmap data stored in a buffer
public struct TerrainHeightSample : IBufferElementData
{
    public float Height;
}

// Tag to identify terrain entities
public struct TerrainTag : IComponentData { }

// Spatial grid for O(1) terrain lookup
public struct TerrainSpatialGrid : IComponentData
{
    public float3 GridOrigin;         // Bottom-left corner of terrain grid
    public float2 CellSize;           // Size of each grid cell (typically one terrain size)
    public int2 GridDimensions;       // Number of cells in X and Z
}

// MonoBehaviour to bake terrain data into ECS
public class TerrainHeightmapBootstrap : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Terrain terrain;

    [Header("Grid Coordinates (for optimization)")]
    [Tooltip("X coordinate in terrain grid (0-based)")]
    public int gridX = 0;

    [Tooltip("Z coordinate in terrain grid (0-based)")]
    public int gridZ = 0;

    [Header("Avoidance Settings")]
    [Tooltip("How high above terrain surface boids should fly")]
    public float minFlightHeight = 3f;

    [Tooltip("Strength of avoidance force")]
    public float avoidanceStrength = 10f;

    [Header("Performance")]
    [Tooltip("Downsample heightmap for performance (1 = full res, 2 = half, 4 = quarter)")]
    [Range(1, 8)]
    public int downsampleFactor = 2;

    private Entity heightmapEntity;
    private static int nextTerrainID = 0;

    void Start()
    {
        if (terrain == null)
        {
            terrain = GetComponent<Terrain>();
            if (terrain == null)
            {
                Debug.LogError("No terrain assigned or found!");
                return;
            }
        }

        BakeTerrainHeightmap();
    }

    void BakeTerrainHeightmap()
    {
        var terrainData = terrain.terrainData;
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Create entity to hold heightmap
        heightmapEntity = em.CreateEntity();

        // Get terrain info
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrainData.size;
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        // Downsample for performance
        int sampledWidth = heightmapWidth / downsampleFactor;
        int sampledHeight = heightmapHeight / downsampleFactor;

        int terrainID = nextTerrainID++;
        Debug.Log($"Baking terrain [{gridX},{gridZ}] ID:{terrainID} heightmap: {sampledWidth}x{sampledHeight} at {terrainPos}");

        // Add component data
        em.AddComponentData(heightmapEntity, new TerrainHeightmapData
        {
            TerrainPosition = terrainPos,
            TerrainSize = terrainSize,
            HeightmapResolution = new int2(sampledWidth, sampledHeight),
            MinFlightHeight = minFlightHeight,
            AvoidanceStrength = avoidanceStrength,
            TerrainID = terrainID,
            GridCoords = new int2(gridX, gridZ)
        });

        // Add terrain tag for querying
        em.AddComponentData(heightmapEntity, new TerrainTag());

        // Add buffer for heightmap samples
        var heightBuffer = em.AddBuffer<TerrainHeightSample>(heightmapEntity);
        heightBuffer.Resize(sampledWidth * sampledHeight, NativeArrayOptions.UninitializedMemory);

        // Sample the heightmap
        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        for (int y = 0; y < sampledHeight; y++)
        {
            for (int x = 0; x < sampledWidth; x++)
            {
                // Sample with downsampling
                int sourceX = x * downsampleFactor;
                int sourceY = y * downsampleFactor;

                // Get height (normalized 0-1) and convert to world height
                float normalizedHeight = heights[sourceY, sourceX];
                float worldHeight = terrainPos.y + (normalizedHeight * terrainSize.y);

                int index = y * sampledWidth + x;
                heightBuffer[index] = new TerrainHeightSample { Height = worldHeight };
            }
        }

        Debug.Log($"Terrain [{gridX},{gridZ}] baked successfully. Entity: {heightmapEntity}");
    }

    void OnDestroy()
    {
        if (heightmapEntity != Entity.Null &&
            World.DefaultGameObjectInjectionWorld != null &&
            World.DefaultGameObjectInjectionWorld.IsCreated)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            if (em.Exists(heightmapEntity))
            {
                em.DestroyEntity(heightmapEntity);
            }
        }
    }
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct TerrainSpatialGridBuilder : ISystem
{
    private bool _gridBuilt;

    public void OnCreate(ref SystemState state)
    {
        _gridBuilt = false;
    }

    public void OnUpdate(ref SystemState state)
    {
        if (_gridBuilt) return;

        var terrainQuery = SystemAPI.QueryBuilder()
            .WithAll<TerrainHeightmapData, TerrainTag>()
            .Build();

        if (terrainQuery.IsEmpty)
            return;

        // Analyze terrain layout to build spatial grid
        var terrainDataArray = terrainQuery.ToComponentDataArray<TerrainHeightmapData>(Allocator.Temp);

        if (terrainDataArray.Length == 0)
        {
            terrainDataArray.Dispose();
            return;
        }

        // Find grid bounds
        int2 minCoords = new int2(int.MaxValue, int.MaxValue);
        int2 maxCoords = new int2(int.MinValue, int.MinValue);
        float3 minPos = new float3(float.MaxValue, float.MaxValue, float.MaxValue);
        float2 cellSize = new float2(terrainDataArray[0].TerrainSize.x, terrainDataArray[0].TerrainSize.z);

        for (int i = 0; i < terrainDataArray.Length; i++)
        {
            var td = terrainDataArray[i];
            minCoords = math.min(minCoords, td.GridCoords);
            maxCoords = math.max(maxCoords, td.GridCoords);
            minPos = math.min(minPos, td.TerrainPosition);
        }

        int2 gridDimensions = maxCoords - minCoords + new int2(1, 1);

        // Create or update spatial grid singleton
        if (SystemAPI.HasSingleton<TerrainSpatialGrid>())
        {
            SystemAPI.SetSingleton(new TerrainSpatialGrid
            {
                GridOrigin = minPos,
                CellSize = cellSize,
                GridDimensions = gridDimensions
            });
        }
        else
        {
            var gridEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(gridEntity, new TerrainSpatialGrid
            {
                GridOrigin = minPos,
                CellSize = cellSize,
                GridDimensions = gridDimensions
            });
        }

        Debug.Log($"Terrain spatial grid built: {gridDimensions.x}x{gridDimensions.y} terrains, " +
                  $"origin: {minPos}, cell size: {cellSize}");

        terrainDataArray.Dispose();
        _gridBuilt = true;

        // Disable this system after building the grid once
        state.Enabled = false;
    }
}
