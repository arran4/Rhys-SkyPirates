using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Enhanced FishFlockController with prefab selection and closest spawn
public partial class FishFlockController : SystemBase
{
    private bool _hasSpawned;
    private NativeHashSet<int> _spawnedFlocks;        // Authoritative baseline completed
    private NativeHashSet<int> _knownFlockCenters;    // Seen in world (event-driven detection)
    private static FishFlockController _instance;

    // STEP 1: Explicit spawn attribution
    private enum SpawnReason
    {
        Startup,
        RuntimeDetection,
        ManualTrigger,
        ManualTriggerClosest
    }

    // STEP 2: Explicit spawn classification
    private enum SpawnType
    {
        Authoritative,
        Additive
    }

    private enum SpawnPhase
    {
        Startup,
        PostStartup
    }

    protected override void OnCreate()
    {
        RequireForUpdate<FishPrefabComponent>();

        _spawnedFlocks = new NativeHashSet<int>(10, Allocator.Persistent);
        _knownFlockCenters = new NativeHashSet<int>(10, Allocator.Persistent);

        _instance = this;

        CleanupDuplicateSettings();
    }

    protected override void OnDestroy()
    {
        if (_spawnedFlocks.IsCreated)
            _spawnedFlocks.Dispose();

        if (_knownFlockCenters.IsCreated)
            _knownFlockCenters.Dispose();

        if (_instance == this)
            _instance = null;
    }

    protected override void OnStartRunning()
    {
        if (_hasSpawned) return;

        SpawnForAllFlockCenters();
        _hasSpawned = true;
    }

    public static FishFlockController Instance => _instance;

    // ---------------- PUBLIC SPAWN TRIGGERS ----------------

    public void TriggerSpawn(int flockId)
    {
        RequestSpawn(flockId, SpawnReason.ManualTrigger);
    }

    public void TriggerSpawnAtClosest(float3 position)
    {
        int closestFlockId = FindClosestFlockCenter(position);
        if (closestFlockId >= 0)
            RequestSpawn(closestFlockId, SpawnReason.ManualTriggerClosest);
        else
            Debug.LogWarning("No flock centers found for closest spawn");
    }

    // ---------------- SPAWN INTENT ENFORCEMENT ----------------

    private void RequestSpawn(int flockId, SpawnReason reason)
    {
        SpawnType type = GetSpawnType(reason);
        SpawnPhase phase = _hasSpawned ? SpawnPhase.PostStartup : SpawnPhase.Startup;

        if (type == SpawnType.Authoritative)
        {
            if (_spawnedFlocks.Contains(flockId))
            {
                Debug.Log(
                    $"[FishFlockController] Authoritative spawn BLOCKED | " +
                    $"FlockID={flockId} | Reason={reason} | Phase={phase}"
                );
                return;
            }

            _spawnedFlocks.Add(flockId);
        }

        Debug.Log(
            $"[FishFlockController] Spawn EXECUTED | " +
            $"FlockID={flockId} | Reason={reason} | Type={type} | Phase={phase}"
        );

        SpawnFish(flockId);
    }

    private static SpawnType GetSpawnType(SpawnReason reason)
    {
        switch (reason)
        {
            case SpawnReason.Startup:
            case SpawnReason.RuntimeDetection:
                return SpawnType.Authoritative;

            case SpawnReason.ManualTrigger:
            case SpawnReason.ManualTriggerClosest:
                return SpawnType.Additive;

            default:
                return SpawnType.Authoritative;
        }
    }

    // ---------------- STARTUP AUTHORITATIVE SPAWN ----------------

    private void SpawnForAllFlockCenters()
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<FlockCenterData, BoidFlockID>()
            .Build();

        if (query.IsEmpty)
        {
            RequestSpawn(0, SpawnReason.Startup);
            _knownFlockCenters.Add(0);
            return;
        }

        var flockIDs = query.ToComponentDataArray<BoidFlockID>(Allocator.Temp);

        for (int i = 0; i < flockIDs.Length; i++)
        {
            int flockId = flockIDs[i].FlockID;
            _knownFlockCenters.Add(flockId);
            RequestSpawn(flockId, SpawnReason.Startup);
        }

        flockIDs.Dispose();
    }

    // ---------------- STEP 4: EVENT-DRIVEN RUNTIME DETECTION ----------------

    protected override void OnUpdate()
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<FlockCenterData, BoidFlockID>()
            .Build();

        if (query.IsEmpty)
            return;

        var flockIDs = query.ToComponentDataArray<BoidFlockID>(Allocator.Temp);

        for (int i = 0; i < flockIDs.Length; i++)
        {
            int flockId = flockIDs[i].FlockID;

            if (_knownFlockCenters.Contains(flockId))
                continue;

            _knownFlockCenters.Add(flockId);
            RequestSpawn(flockId, SpawnReason.RuntimeDetection);
        }

        flockIDs.Dispose();
    }

    // ---------------- EXISTING SPAWN LOGIC (AUTHORITATIVE SETTINGS RESOLUTION) ----------------

    private void SpawnFish(int flockId)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var availablePrefabs = GetPrefabsForFlock(flockId);
        if (availablePrefabs.Length == 0)
        {
            Debug.LogError($"No prefabs available for FlockID {flockId}");
            availablePrefabs.Dispose();
            return;
        }

        FishControllerSettingsComponent settings = ResolveSettingsForSpawn();

        float3 spawnCenter = GetFlockCenterPosition(flockId, settings.BoundaryCenter);
        var random = new Unity.Mathematics.Random(
            (uint)(UnityEngine.Random.Range(1, int.MaxValue) + flockId * 1000));

        for (int i = 0; i < settings.SpawnCount; i++)
        {
            Entity instance = entityManager.Instantiate(
                availablePrefabs[random.NextInt(0, availablePrefabs.Length)]);

            if (entityManager.HasComponent<SceneTag>(instance))
                entityManager.RemoveComponent<SceneTag>(instance);

            if (entityManager.HasBuffer<LinkedEntityGroup>(instance))
            {
                var buffer = entityManager.GetBuffer<LinkedEntityGroup>(instance);
                for (int j = 0; j < buffer.Length; j++)
                {
                    var child = buffer[j].Value;
                    if (entityManager.HasComponent<SceneTag>(child))
                        entityManager.RemoveComponent<SceneTag>(child);
                }
            }

            float3 forward = random.NextFloat3Direction();
            float3 pos = spawnCenter + forward * random.NextFloat(0f, settings.SpawnRadius);

            entityManager.SetComponentData(instance, new LocalTransform
            {
                Position = pos,
                Rotation = quaternion.LookRotationSafe(forward, math.up()),
                Scale = 1f
            });

            entityManager.SetComponentData(instance,
                new Velocity { Value = forward * settings.InitialSpeed });

            entityManager.SetComponentData(instance, new BoidSettings
            {
                MaxSpeed = settings.MaxSpeed,
                SearchRadius = settings.SearchRadius,
                ObstacleAvoidanceDistance = settings.ObstacleAvoidanceDistance,
                ObstacleAvoidanceWeight = settings.ObstacleAvoidanceWeight,
                CohesionWeight = settings.CohesionWeight,
                AlignmentWeight = settings.AlignmentWeight,
                SeparationWeight = settings.SeparationWeight
            });

            entityManager.SetComponentData(instance, new BoundarySettings
            {
                Center = spawnCenter,
                Radius = settings.BoundaryRadius,
                BoundaryWeight = settings.BoundaryWeight
            });

            entityManager.AddComponent<BoidTag>(instance);
            entityManager.AddComponentData(instance,
                new BoidFlockID { FlockID = flockId });
        }

        Debug.Log($"Spawned {settings.SpawnCount} boids for FlockID {flockId} at {spawnCenter}");
        availablePrefabs.Dispose();
    }

    // ---------------- SETTINGS AUTHORITY (AUTHORITATIVE AT SPAWN) ----------------

    private FishControllerSettingsComponent ResolveSettingsForSpawn()
    {
        var settingsQuery = EntityManager.CreateEntityQuery(typeof(FishControllerSettingsComponent));
        int settingsCount = settingsQuery.CalculateEntityCount();

        Debug.Log($"[FishFlockController] ResolveSettingsForSpawn: found {settingsCount} settings entities");

        if (settingsCount > 1)
        {
            var entities = settingsQuery.ToEntityArray(Allocator.Temp);
            for (int i = 1; i < entities.Length; i++)
            {
                Debug.Log($"[FishFlockController] Destroying duplicate settings entity {entities[i]}");
                EntityManager.DestroyEntity(entities[i]);
            }
            entities.Dispose();
            settingsCount = 1;
        }

        FishControllerSettingsComponent settings;

        if (settingsCount > 0)
        {
            var arr = settingsQuery.ToComponentDataArray<FishControllerSettingsComponent>(Allocator.Temp);
            settings = arr[0];
            arr.Dispose();
            Debug.Log("[FishFlockController] Using resolved FishControllerSettingsComponent");
        }
        else
        {
            settings = GetDefaultSettings();
            Debug.Log("[FishFlockController] No settings entity found — using DEFAULT settings");
        }

        settingsQuery.Dispose();
        return settings;
    }

    // ---------------- HELPERS (UNCHANGED) ----------------

    private int FindClosestFlockCenter(float3 position)
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<FlockCenterData, BoidFlockID>()
            .Build();

        if (query.IsEmpty) return -1;

        var ids = query.ToComponentDataArray<BoidFlockID>(Allocator.Temp);
        var centers = query.ToComponentDataArray<FlockCenterData>(Allocator.Temp);

        float best = float.MaxValue;
        int result = -1;

        for (int i = 0; i < centers.Length; i++)
        {
            float d = math.lengthsq(position - centers[i].Position);
            if (d < best)
            {
                best = d;
                result = ids[i].FlockID;
            }
        }

        ids.Dispose();
        centers.Dispose();
        return result;
    }

    private NativeArray<Entity> GetPrefabsForFlock(int flockId)
    {
        var all = GetAllAvailablePrefabs();

        var query = SystemAPI.QueryBuilder()
            .WithAll<FlockCenterData, BoidFlockID>()
            .Build();

        if (query.IsEmpty)
            return all;

        var entities = query.ToEntityArray(Allocator.Temp);
        var ids = query.ToComponentDataArray<BoidFlockID>(Allocator.Temp);

        Entity center = Entity.Null;
        for (int i = 0; i < ids.Length; i++)
            if (ids[i].FlockID == flockId)
                center = entities[i];

        entities.Dispose();
        ids.Dispose();

        if (center == Entity.Null || !EntityManager.HasBuffer<FlockPrefabSelection>(center))
            return all;

        var buffer = EntityManager.GetBuffer<FlockPrefabSelection>(center);
        var selected = new NativeList<Entity>(Allocator.Temp);

        foreach (var entry in buffer)
        {
            if (entry.PrefabEntity != Entity.Null)
                selected.Add(entry.PrefabEntity);
            else if (entry.PrefabIndex == -1)
            {
                selected.AddRange(all);
                break;
            }
            else if (entry.PrefabIndex >= 0 && entry.PrefabIndex < all.Length)
                selected.Add(all[entry.PrefabIndex]);
        }

        if (selected.Length > 0)
        {
            var result = new NativeArray<Entity>(selected.Length, Allocator.Temp);
            for (int i = 0; i < selected.Length; i++)
                result[i] = selected[i];

            selected.Dispose();
            all.Dispose();
            return result;
        }

        selected.Dispose();
        return all;
    }

    private NativeArray<Entity> GetAllAvailablePrefabs()
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<FishPrefabReference>()
            .Build();

        if (!query.IsEmpty)
        {
            var e = query.GetSingletonEntity();
            var buffer = EntityManager.GetBuffer<FishPrefabReference>(e);

            if (buffer.Length > 0)
            {
                var arr = new NativeArray<Entity>(buffer.Length, Allocator.Temp);
                for (int i = 0; i < buffer.Length; i++)
                    arr[i] = buffer[i].Prefab;
                return arr;
            }
        }

        if (SystemAPI.HasSingleton<FishPrefabComponent>())
        {
            var arr = new NativeArray<Entity>(1, Allocator.Temp);
            arr[0] = SystemAPI.GetSingleton<FishPrefabComponent>().prefab;
            return arr;
        }

        return new NativeArray<Entity>(0, Allocator.Temp);
    }

    private float3 GetFlockCenterPosition(int flockId, float3 fallback)
    {
        var query = SystemAPI.QueryBuilder()
            .WithAll<FlockCenterData, BoidFlockID>()
            .Build();

        if (!query.IsEmpty)
        {
            var ids = query.ToComponentDataArray<BoidFlockID>(Allocator.Temp);
            var centers = query.ToComponentDataArray<FlockCenterData>(Allocator.Temp);

            for (int i = 0; i < ids.Length; i++)
                if (ids[i].FlockID == flockId)
                {
                    var pos = centers[i].Position;
                    ids.Dispose();
                    centers.Dispose();
                    return pos;
                }

            ids.Dispose();
            centers.Dispose();
        }

        return fallback;
    }

    private FishControllerSettingsComponent GetDefaultSettings()
    {
        return new FishControllerSettingsComponent
        {
            SpawnCount = 500,
            SpawnRadius = 10f,
            InitialSpeed = 2f,
            MaxSpeed = 5f,
            SearchRadius = 5f,
            CohesionWeight = 1f,
            AlignmentWeight = 1f,
            SeparationWeight = 1f,
            ObstacleAvoidanceDistance = 2f,
            ObstacleAvoidanceWeight = 1f,
            BoundaryCenter = float3.zero,
            BoundaryRadius = 20f,
            BoundaryWeight = 2f
        };
    }

    void CleanupDuplicateSettings()
    {
        var settingsQuery = GetEntityQuery(typeof(FishControllerSettingsComponent));
        var settings = settingsQuery.ToEntityArray(Allocator.Temp);

        Debug.Log($"[FishFlockController] CleanupDuplicateSettings: found {settings.Length} settings entities");

        if (settings.Length > 1)
        {
            for (int i = 1; i < settings.Length; i++)
            {
                Debug.Log($"[FishFlockController] Destroying duplicate FishControllerSettings entity {settings[i]}");
                EntityManager.DestroyEntity(settings[i]);
            }

            Debug.Log($"[FishFlockController] Settings authority resolved to entity {settings[0]}");
        }
        else if (settings.Length == 1)
        {
            Debug.Log($"[FishFlockController] Single settings entity present: {settings[0]}");
        }
        else
        {
            Debug.Log("[FishFlockController] No FishControllerSettingsComponent found (defaults will be used later)");
        }

        settings.Dispose();
    }
}

public struct OverworldTag : IComponentData { }
