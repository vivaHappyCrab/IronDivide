using System;
using IronDivide.Core.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace IronDivide.Core.Systems
{
    [BurstCompile]
    public partial struct TrainMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var et=SystemAPI.Time.ElapsedTime;
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<TrainElement>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    TrainElement element = buffer[i];
                    ref var train = ref SystemAPI.GetComponentRW<Train>(element.Train).ValueRW;
                    train.Position=new float2((float)Math.Cos(et),(float)Math.Sin(et));
                }
            }
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}