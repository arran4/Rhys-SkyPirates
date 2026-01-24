using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Struct definitions from original system
public struct BoidUpdateGroup : IComponentData { public int Group; }
public struct CachedShipData : IComponentData { public ShipData Data; }
public struct ShipData { public bool HasShip; public float3 Position; public float Radius; }

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(OptimizedSpatialHashSystem))]
[UpdateAfter(typeof(AssignBoidUpdateGroupsSystem))]
public partial struct FlockingBehaviorSystem : ISystem
{
    private double _lastShipUpdateTime;
    private int _currentUpdateGroup;
    private ComponentLookup<BoidUpdateGroup> _boidGroupLookup;
    private ComponentLookup<TerrainHeightmapData> _terrainDataLookup;
    private BufferLookup<TerrainHeightSample> _terrainBufferLookup;
    private EntityQuery _flockCenterQuery;
    private EntityQuery _terrainQuery;

    public void OnCreate(ref SystemState state)
    {
        _boidGroupLookup = state.GetComponentLookup<BoidUpdateGroup>(isReadOnly: true);
        _terrainDataLookup = state.GetComponentLookup<TerrainHeightmapData>(isReadOnly: true);
        _terrainBufferLookup = state.GetBufferLookup<TerrainHeightSample>(isReadOnly: true);
        _flockCenterQuery = state.GetEntityQuery(ComponentType.ReadOnly<FlockCenterData>());

        _terrainQuery = state.GetEntityQuery(
            ComponentType.ReadOnly<TerrainHeightmapData>(),
            ComponentType.ReadOnly<TerrainTag>()
        );
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        double currentTime = SystemAPI.Time.ElapsedTime;
        int frameCount = Time.frameCount;

        var spatialHashSystem = state.WorldUnmanaged.GetExistingUnmanagedSystem<OptimizedSpatialHashSystem>();
        var spatialHashRef = state.WorldUnmanaged.GetUnsafeSystemRef<OptimizedSpatialHashSystem>(spatialHashSystem);
        var spatialMap = spatialHashRef.GetSpatialMap();

        if (spatialHashRef.GetLastBoidCount() == 0) return;

        // Build terrain grid lookup
        bool hasTerrainData = !_terrainQuery.IsEmpty;
        NativeParallelMultiHashMap<int2, Entity> terrainGrid = default;
        TerrainSpatialGrid spatialGrid = default;

        if (hasTerrainData)
        {
            var terrainEntities = _terrainQuery.ToEntityArray(Allocator.TempJob);
            var terrainDataArray = _terrainQuery.ToComponentDataArray<TerrainHeightmapData>(Allocator.TempJob);

            terrainGrid = new NativeParallelMultiHashMap<int2, Entity>(terrainEntities.Length, Allocator.TempJob);

            for (int i = 0; i < terrainDataArray.Length; i++)
            {
                terrainGrid.Add(terrainDataArray[i].GridCoords, terrainEntities[i]);
            }

            terrainEntities.Dispose();
            terrainDataArray.Dispose();

            // Get spatial grid if available
            var gridQuery = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<TerrainSpatialGrid>());
            if (!gridQuery.IsEmpty)
            {
                spatialGrid = gridQuery.GetSingleton<TerrainSpatialGrid>();
            }
            gridQuery.Dispose();
        }

        // Ship avoidance caching
        var shipData = new ShipData { HasShip = false };
        if (currentTime - _lastShipUpdateTime > 0.2)
        {
            foreach (var (transform, tag) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<ShipProxyTag>>())
            {
                shipData.HasShip = true;
                shipData.Position = transform.ValueRO.Position;
                shipData.Radius = 1f;
                _lastShipUpdateTime = currentTime;
                break;
            }

            var shipQuery = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<CachedShipData>());
            if (!shipQuery.IsEmpty)
            {
                var shipEntity = shipQuery.GetSingletonEntity();
                state.EntityManager.SetComponentData(shipEntity, new CachedShipData { Data = shipData });
            }
            else
            {
                var shipEntity = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(shipEntity, new CachedShipData { Data = shipData });
            }
            shipQuery.Dispose();
        }
        else
        {
            var shipQuery = state.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<CachedShipData>());
            if (!shipQuery.IsEmpty)
            {
                shipData = shipQuery.GetSingleton<CachedShipData>().Data;
            }
            shipQuery.Dispose();
        }

        // Build flock centers map
        int centerCount = _flockCenterQuery.CalculateEntityCount();
        var flockCenters = new NativeParallelHashMap<int, float3>(math.max(1, centerCount), Allocator.TempJob);

        if (centerCount > 0)
        {
            foreach (var fc in SystemAPI.Query<RefRO<FlockCenterData>>())
            {
                flockCenters.TryAdd(fc.ValueRO.FlockID, fc.ValueRO.Position);
            }
        }

        _currentUpdateGroup = frameCount % 4;

        var boidQuery = SystemAPI.QueryBuilder()
            .WithAll<BoidTag, LocalTransform, Velocity, BoidSettings, BoundarySettings, BoidFlockID>()
            .Build();

        _boidGroupLookup.Update(ref state);
        _terrainDataLookup.Update(ref state);
        _terrainBufferLookup.Update(ref state);

        var flockingJob = new FlockingJob
        {
            SpatialMap = spatialMap,
            DeltaTime = deltaTime * 4f,
            ShipData = shipData,
            FrameCount = frameCount,
            CurrentUpdateGroup = _currentUpdateGroup,
            BoidUpdateGroupHandle = _boidGroupLookup,
            FlockCenters = flockCenters,
            HasTerrainData = hasTerrainData,
            TerrainGrid = terrainGrid,
            SpatialGrid = spatialGrid,
            TerrainDataLookup = _terrainDataLookup,
            TerrainBufferLookup = _terrainBufferLookup
        };

        state.Dependency = flockingJob.ScheduleParallel(boidQuery, state.Dependency);
        state.Dependency = flockCenters.Dispose(state.Dependency);

        if (hasTerrainData)
        {
            state.Dependency = terrainGrid.Dispose(state.Dependency);
        }
    }
}

[BurstCompile]
public partial struct FlockingJob : IJobEntity
{
    [ReadOnly] public NativeParallelMultiHashMap<int3, OptimizedSpatialHashSystem.BoidData> SpatialMap;
    [ReadOnly] public float DeltaTime;
    [ReadOnly] public ShipData ShipData;
    [ReadOnly] public int FrameCount;
    [ReadOnly] public int CurrentUpdateGroup;
    [ReadOnly] public ComponentLookup<BoidUpdateGroup> BoidUpdateGroupHandle;
    [ReadOnly] public NativeParallelHashMap<int, float3> FlockCenters;

    [ReadOnly] public bool HasTerrainData;
    [ReadOnly] public NativeParallelMultiHashMap<int2, Entity> TerrainGrid;
    [ReadOnly] public TerrainSpatialGrid SpatialGrid;
    [ReadOnly] public ComponentLookup<TerrainHeightmapData> TerrainDataLookup;
    [ReadOnly] public BufferLookup<TerrainHeightSample> TerrainBufferLookup;

    void Execute(Entity entity, [EntityIndexInQuery] int index,
        ref LocalTransform transform, ref Velocity velocity,
        in BoidSettings settings, in BoundarySettings boundarySettings, in BoidFlockID flockId)
    {
        int group;
        if (BoidUpdateGroupHandle.HasComponent(entity))
        {
            group = BoidUpdateGroupHandle[entity].Group;
        }
        else
        {
            group = index % 4;
        }

        if (group != CurrentUpdateGroup) return;

        float3 pos = transform.Position;
        if (math.any(math.isnan(pos))) return;

        float3 targetCenter = boundarySettings.Center;
        if (FlockCenters.TryGetValue(flockId.FlockID, out var mappedCenter))
        {
            targetCenter = mappedCenter;
        }

        float3 zero = float3.zero;
        float3 up = new float3(0, 1, 0);
        float3 steer = zero;
        float3 obstacle = zero;
        float3 boundaryForce = zero;

        var neighbors = new NativeArray<OptimizedSpatialHashSystem.BoidData>(48, Allocator.Temp);
        int neighborCount = GetNeighbors(pos, settings.SearchRadius, neighbors);
        float3 alignment = zero;
        float3 cohesion = zero;
        float3 separation = zero;
        int valid = 0;
        float radiusSq = settings.SearchRadius * settings.SearchRadius;

        for (int i = 0; i < neighborCount; i++)
        {
            var neighbor = neighbors[i];
            if (neighbor.Entity == entity) continue;

            float3 diff = pos - neighbor.Position;
            float distSq = math.lengthsq(diff);
            if (distSq > radiusSq || distSq < 0.0001f) continue;

            cohesion += neighbor.Position;
            alignment += neighbor.Velocity;
            separation += diff / (distSq + 0.01f);
            valid++;
        }

        if (valid > 0)
        {
            float inv = 1f / valid;
            steer =
                math.normalizesafe(cohesion * inv - pos) * settings.CohesionWeight +
                math.normalizesafe(alignment * inv - velocity.Value) * settings.AlignmentWeight +
                math.normalizesafe(separation) * settings.SeparationWeight;
        }

        bool avoiding = false;
        if (ShipData.HasShip && settings.ObstacleAvoidanceDistance > 0.001f)
        {
            float3 diff = pos - ShipData.Position;
            float distSq = math.lengthsq(diff);
            float avoidRadius = ShipData.Radius + settings.ObstacleAvoidanceDistance;
            float avoidRadiusSq = avoidRadius * avoidRadius;

            if (distSq < avoidRadiusSq && distSq > 0.000001f)
            {
                avoiding = true;
                float invDist = math.rsqrt(distSq);
                float strength = (avoidRadius - math.sqrt(distSq)) / avoidRadius;
                obstacle = diff * invDist * settings.ObstacleAvoidanceWeight * strength * 4f;
            }
        }

        float3 terrainAvoidance = zero;
        if (HasTerrainData)
        {
            terrainAvoidance = CalculateTerrainAvoidance(pos, velocity.Value);
        }

        boundaryForce = CalculateBoundaryForce(pos, targetCenter, boundarySettings);

        float3 totalForce = avoiding
            ? obstacle * 5f + steer * 0.3f + boundaryForce * 0.5f + terrainAvoidance * 0.3f
            : steer + boundaryForce + terrainAvoidance;

        float3 newVel = velocity.Value + totalForce * DeltaTime;

        float speedSq = math.lengthsq(newVel);
        float maxSpeed = settings.MaxSpeed;
        float maxSpeedSq = maxSpeed * maxSpeed;
        float minSpeed = maxSpeed * 0.3f;
        float minSpeedSq = minSpeed * minSpeed;

        if (speedSq > maxSpeedSq)
            newVel *= maxSpeed * math.rsqrt(speedSq);
        else if (speedSq < minSpeedSq && speedSq > 0.000001f)
            newVel *= minSpeed * math.rsqrt(speedSq);

        velocity.Value = newVel;

        if (speedSq > 0.001f)
        {
            float3 dir = math.normalizesafe(newVel);
            if (math.lengthsq(dir) > 0.0001f && !math.any(math.isnan(dir)))
            {
                transform.Rotation = quaternion.LookRotationSafe(dir, up);
            }
        }

        transform.Position += newVel * DeltaTime;
        neighbors.Dispose();
    }

    [BurstCompile]
    private float3 CalculateTerrainAvoidance(float3 pos, float3 vel)
    {
        float3 localPos = pos - SpatialGrid.GridOrigin;
        int2 gridCoords = new int2(
            (int)math.floor(localPos.x / SpatialGrid.CellSize.x),
            (int)math.floor(localPos.z / SpatialGrid.CellSize.y)
        );

        if (gridCoords.x < 0 || gridCoords.x >= SpatialGrid.GridDimensions.x ||
            gridCoords.y < 0 || gridCoords.y >= SpatialGrid.GridDimensions.y)
        {
            return float3.zero;
        }

        if (!TerrainGrid.TryGetFirstValue(gridCoords, out Entity terrainEntity, out var iterator))
        {
            return float3.zero;
        }

        var terrainData = TerrainDataLookup[terrainEntity];
        var heightBuffer = TerrainBufferLookup[terrainEntity];
        float terrainHeight = SampleTerrainHeight(pos, terrainData, heightBuffer);

        float heightAboveTerrain = pos.y - terrainHeight;

        if (heightAboveTerrain >= terrainData.MinFlightHeight)
            return float3.zero;

        float3 avoidanceForce = float3.zero;

        if (heightAboveTerrain < 0)
        {
            float penetration = -heightAboveTerrain;
            float urgency = math.min(penetration / 2f, 5f);
            avoidanceForce = new float3(0, 1, 0) * terrainData.AvoidanceStrength * (2f + urgency);
        }
        else
        {
            float normalizedHeight = heightAboveTerrain / terrainData.MinFlightHeight;
            float strength = (1f - normalizedHeight) * (1f - normalizedHeight);
            avoidanceForce = new float3(0, 1, 0) * terrainData.AvoidanceStrength * strength;

            if (vel.y < 0)
            {
                avoidanceForce.y += -vel.y * strength * 2f;
            }
        }

        float3 lookAheadPos = pos + math.normalizesafe(vel) * 3f;
        float3 lookAheadLocal = lookAheadPos - SpatialGrid.GridOrigin;
        int2 lookAheadGrid = new int2(
            (int)math.floor(lookAheadLocal.x / SpatialGrid.CellSize.x),
            (int)math.floor(lookAheadLocal.z / SpatialGrid.CellSize.y)
        );

        if (lookAheadGrid.x >= 0 && lookAheadGrid.x < SpatialGrid.GridDimensions.x &&
            lookAheadGrid.y >= 0 && lookAheadGrid.y < SpatialGrid.GridDimensions.y)
        {
            if (TerrainGrid.TryGetFirstValue(lookAheadGrid, out Entity lookAheadTerrain, out var _))
            {
                var lookAheadTerrainData = TerrainDataLookup[lookAheadTerrain];
                var lookAheadHeightBuffer = TerrainBufferLookup[lookAheadTerrain];
                float lookAheadHeight = SampleTerrainHeight(lookAheadPos, lookAheadTerrainData, lookAheadHeightBuffer);
                float lookAheadClearance = lookAheadPos.y - lookAheadHeight;

                if (lookAheadClearance < terrainData.MinFlightHeight * 0.5f)
                {
                    avoidanceForce += new float3(0, 1, 0) * terrainData.AvoidanceStrength * 0.5f;
                }
            }
        }

        return avoidanceForce;
    }

    [BurstCompile]
    private float SampleTerrainHeight(float3 worldPos, TerrainHeightmapData terrainData, DynamicBuffer<TerrainHeightSample> heightSamples)
    {
        float3 localPos = worldPos - terrainData.TerrainPosition;

        float normalizedX = localPos.x / terrainData.TerrainSize.x;
        float normalizedZ = localPos.z / terrainData.TerrainSize.z;

        normalizedX = math.clamp(normalizedX, 0f, 1f);
        normalizedZ = math.clamp(normalizedZ, 0f, 1f);

        float heightmapX = normalizedX * (terrainData.HeightmapResolution.x - 1);
        float heightmapZ = normalizedZ * (terrainData.HeightmapResolution.y - 1);

        int x0 = (int)math.floor(heightmapX);
        int z0 = (int)math.floor(heightmapZ);
        int x1 = math.min(x0 + 1, terrainData.HeightmapResolution.x - 1);
        int z1 = math.min(z0 + 1, terrainData.HeightmapResolution.y - 1);

        float tx = heightmapX - x0;
        float tz = heightmapZ - z0;

        float h00 = heightSamples[z0 * terrainData.HeightmapResolution.x + x0].Height;
        float h10 = heightSamples[z0 * terrainData.HeightmapResolution.x + x1].Height;
        float h01 = heightSamples[z1 * terrainData.HeightmapResolution.x + x0].Height;
        float h11 = heightSamples[z1 * terrainData.HeightmapResolution.x + x1].Height;

        float h0 = math.lerp(h00, h10, tx);
        float h1 = math.lerp(h01, h11, tx);
        float finalHeight = math.lerp(h0, h1, tz);

        return finalHeight;
    }

    [BurstCompile]
    private int GetNeighbors(float3 pos, float radius, NativeArray<OptimizedSpatialHashSystem.BoidData> buffer)
    {
        float cellSize = 5f;

        // Inline the spatial hash calculation to avoid Burst errors
        int3 center = new int3(
            (int)math.floor(pos.x / cellSize),
            (int)math.floor(pos.y / cellSize),
            (int)math.floor(pos.z / cellSize)
        );

        float radiusSq = radius * radius;
        int count = 0;

        int radiusInCells = (int)math.ceil(radius / cellSize);
        radiusInCells = math.max(1, radiusInCells);

        for (int x = -radiusInCells; x <= radiusInCells; x++)
            for (int y = -radiusInCells; y <= radiusInCells; y++)
                for (int z = -radiusInCells; z <= radiusInCells; z++)
                {
                    int3 cell = center + new int3(x, y, z);
                    if (SpatialMap.TryGetFirstValue(cell, out var data, out var iter))
                    {
                        do
                        {
                            if (count >= buffer.Length) return count;
                            float distSq = math.lengthsq(pos - data.Position);
                            if (distSq <= radiusSq) buffer[count++] = data;
                        }
                        while (SpatialMap.TryGetNextValue(out data, ref iter));
                    }
                }

        return count;
    }

    [BurstCompile]
    private float3 CalculateBoundaryForce(float3 pos, float3 center, BoundarySettings boundary)
    {
        float3 diff = center - pos;
        float distSq = math.lengthsq(diff);
        float innerRadiusSq = boundary.Radius * boundary.Radius * 0.49f;

        if (distSq > innerRadiusSq)
        {
            float dist = math.sqrt(distSq);
            float strength = math.saturate((dist - boundary.Radius * 0.7f) / (boundary.Radius * 0.3f));
            return math.normalizesafe(diff) * boundary.BoundaryWeight * strength;
        }

        return float3.zero;
    }
}


[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct AssignBoidUpdateGroupsSystem : ISystem
{
    private ComponentLookup<BoidUpdateGroup> _boidGroupLookup;

    public void OnCreate(ref SystemState state)
    {
        _boidGroupLookup = state.GetComponentLookup<BoidUpdateGroup>(isReadOnly: false);
    }

    public void OnUpdate(ref SystemState state)
    {
        _boidGroupLookup.Update(ref state);

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        int groupCounter = 0;

        foreach (var (tag, entity) in SystemAPI.Query<RefRO<BoidTag>>().WithEntityAccess().WithNone<BoidUpdateGroup>())
        {
            ecb.AddComponent(entity, new BoidUpdateGroup { Group = groupCounter % 4 });
            groupCounter++;
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

        var ungroupedQuery = SystemAPI.QueryBuilder().WithAll<BoidTag>().WithNone<BoidUpdateGroup>().Build();
        if (ungroupedQuery.IsEmpty)
        {
            state.Enabled = false;
        }
    }
}
