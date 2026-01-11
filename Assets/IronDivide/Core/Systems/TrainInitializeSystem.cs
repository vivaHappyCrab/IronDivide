using System;
using IronDivide.Core.Components;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.UI;

namespace IronDivide.Core.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct TrainInitializeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldState>();
            state.RequireForUpdate<IronDivide.Core.Components.Grid>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var worldState = SystemAPI.GetSingleton<WorldState>();
            var worldStateEntity = SystemAPI.GetSingletonEntity<WorldState>();
            UnityEngine.Debug.Log($"{worldStateEntity.ToFixedString()}");
            var grid = SystemAPI.GetSingleton<IronDivide.Core.Components.Grid>();
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                             .CreateCommandBuffer(state.WorldUnmanaged);

            var train = ecb.CreateEntity();
            var trainComponent = new Train
            {
                Id = Convert.ToUInt64(worldState.Seed),
                Name = "qq",
                Position = new float2(0 * grid.cellWidth, 1 * grid.cellHeight)
            };
            ecb.AddComponent<Train>(train, trainComponent);

            var buffer = ecb.AddBuffer<TrainElement>(worldStateEntity);
            buffer.Add(new TrainElement{Train=train});
            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}