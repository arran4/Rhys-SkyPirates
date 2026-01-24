using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public static class BoidInstantiation
{
    public static void InstantiateBoids(
        EntityManager entityManager,
        int flockId,
        NativeArray<Entity> prefabs,
        FishControllerSettingsComponent settings,
        float3 spawnCenter,
        Unity.Mathematics.Random random)
    {
        for (int i = 0; i < settings.SpawnCount; i++)
        {
            Entity instance = entityManager.Instantiate(
                prefabs[random.NextInt(0, prefabs.Length)]);

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
            float3 position = spawnCenter + forward * random.NextFloat(0f, settings.SpawnRadius);

            entityManager.SetComponentData(instance, new LocalTransform
            {
                Position = position,
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
    }
}
