using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using IronDivide.Core.Components;
using UnityEngine;

namespace IronDivide.Core.Systems
{
    [BurstCompile]
    public partial struct TrainVisualizeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PrefabStorage>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                 .CreateCommandBuffer(state.WorldUnmanaged);
            var prefab = SystemAPI.ManagedAPI.GetSingleton<PrefabStorage>();
            foreach ((var train, var entity) in SystemAPI.Query<Train>().WithNone<TrainVisual>().WithEntityAccess())
            {
                var trainObject = Object.Instantiate(prefab.Train);
                ecb.AddComponent<TrainVisual>(entity, new TrainVisual
                {
                    TrainVisualObject = trainObject
                });
                trainObject.transform.position = new Vector3(train.Position.x, 0.5f, train.Position.y);
            }

            foreach ((var train, var visual) in SystemAPI.Query<Train,TrainVisual>())
            {
                visual.TrainVisualObject.transform.position = new Vector3(train.Position.x, 0.5f, train.Position.y);
            }
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}