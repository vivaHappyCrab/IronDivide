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
            var grid = SystemAPI.GetSingleton<IronDivide.Core.Components.Grid>();
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                             .CreateCommandBuffer(state.WorldUnmanaged);

            var station1 = ecb.CreateEntity();
            var station1Comp = new Station
            {
                Id = 1,
                Name = "Загорье",
                Position = new float2(10, 0)
            };
            ecb.AddComponent<Station>(station1, station1Comp);

            var station2 = ecb.CreateEntity();
            var station2Comp = new Station
            {
                Id = 1,
                Name = "Мак на 6ой радиальной",
                Position = new float2(0, 10)
            };
            ecb.AddComponent<Station>(station2, station2Comp);

            var stationBuffer = ecb.AddBuffer<StationElement>(worldStateEntity);
            stationBuffer.Add(new StationElement { Station = station1 });
            stationBuffer.Add(new StationElement { Station = station2 });


            var train = ecb.CreateEntity();
            var trainComponent = new Train
            {
                Id = 1,
                Name = "901",
                Position = station1Comp.Position,
                Target = station2,
                MaxSpeed = 20f
            };
            ecb.AddComponent<Train>(train, trainComponent);

            var trainBuffer = ecb.AddBuffer<TrainElement>(worldStateEntity);
            trainBuffer.Add(new TrainElement { Train = train });

            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}