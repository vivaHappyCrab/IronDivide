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
        private const float ContactDistance = 0.01f;

        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var et = SystemAPI.Time.ElapsedTime;
            var dt = SystemAPI.Time.DeltaTime;
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<TrainElement>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    TrainElement element = buffer[i];
                    ref var train = ref SystemAPI.GetComponentRW<Train>(element.Train).ValueRW;
                    if (train.WaitTime > 0)
                    {
                        train.WaitTime -= dt;
                        continue;
                    }
                    var targetStation = SystemAPI.GetComponent<Station>(train.Target);
                    if (train.Target == default) continue;

                    if ((train.Position.x - targetStation.Position.x) * (train.Position.x - targetStation.Position.x)
                        + (train.Position.y - targetStation.Position.y) * (train.Position.y - targetStation.Position.y)
                        > ContactDistance * ContactDistance)
                    {
                        if (train.Speed < train.MaxSpeed)
                            train.Speed = Math.Min(train.Speed + 5f * dt, train.MaxSpeed);
                        train.Position = CalculateNewPosition(train.Position, targetStation.Position, train.Speed * dt);
                    }
                    else
                    {
                        train.Speed = 0f;
                        train.WaitTime = 3f;
                        train.Position = targetStation.Position;
                        train.Target = SelectNewTarget(train, ref state);
                    }
                }
            }
        }

        public void OnDestroy(ref SystemState state)
        {

        }

        private float2 CalculateNewPosition(float2 from, float2 to, float ds)
        {
            var distance = (to.x - from.x) * (to.x - from.x) + (to.y - from.y) * (to.y - from.y);
            if (ds > distance)
                ds = distance;
            var newX = from.x + (to.x - from.x) * ds / distance;
            var newY = from.y + (to.y - from.y) * ds / distance;
            return new float2(newX, newY);
        }

        private Entity SelectNewTarget(Train train, ref SystemState state)
        {
            foreach (var buffer in SystemAPI.Query<DynamicBuffer<StationElement>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (train.Target != buffer[i].Station)
                        return buffer[i].Station;
                }
                return buffer[0].Station;
            }
            return default;
        }
    }
}