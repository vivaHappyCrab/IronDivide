using System;
using IronDivide.Core.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Splines;

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
            // foreach (var buffer in SystemAPI.Query<DynamicBuffer<TrainElement>>())
            // {
            //     for (int i = 0; i < buffer.Length; i++)
            //         //ProcessTrain(buffer[i].Train,ref state,dt);
            // }

            foreach (var buffer in SystemAPI.Query<DynamicBuffer<StationElement>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                    ProcessStation(buffer[i].Station, ref state, dt);
            }

            foreach (var buffer in SystemAPI.Query<DynamicBuffer<TrackElement>>())
            {
                for (int i = 0; i < buffer.Length; i++)
                    ProcessTrack(buffer[i].Track, ref state, dt);
            }
            //TODO: процессим трэки и двигаем по ним поезда
        }

        public void OnDestroy(ref SystemState state)
        {

        }

        private void ProcessStation(Entity stationEntity, ref SystemState state, float dt)
        {
            if (!SystemAPI.HasBuffer<TrainElement>(stationEntity))
                return;
            var trainsOnStation = SystemAPI.GetBuffer<TrainElement>(stationEntity);
            for (int i = 0; i < trainsOnStation.Length; i++)
            {
                var trainEntity = trainsOnStation[i].Train;
                ref var train = ref SystemAPI.GetComponentRW<Train>(trainEntity).ValueRW;
                if (train.State == TrainState.Waiting)
                {
                    train.WaitTime -= dt;
                    if (train.WaitTime <= 0)
                        train.State = state.EntityManager.Exists(train.Order) ? TrainState.Idle : TrainState.OnWay;
                    //TODO:выпустить на рельсы
                }
            }
        }

        private void ProcessTrack(Entity trackEntity, ref SystemState state, float dt)
        {
            UnityEngine.Debug.Log($"Processing track {trackEntity.ToFixedString()}");
            if (!SystemAPI.HasBuffer<TrainOnTrackElement>(trackEntity))
                return;

            var track = SystemAPI.GetComponent<Track>(trackEntity);
            var trackSpline = SystemAPI.GetComponent<SplineBlobAssetComponent>(trackEntity).reference.Value.CreateNativeSpline(Unity.Collections.Allocator.Temp);
            var trainsOnTrack = SystemAPI.GetBuffer<TrainOnTrackElement>(trackEntity);
            for (int i = 0; i < trainsOnTrack.Length; i++)
            {
                var trainOnTrack = trainsOnTrack[i];
                UnityEngine.Debug.Log($"Processing train {trainOnTrack.Train.ToFixedString()}");
                ref var train = ref SystemAPI.GetComponentRW<Train>(trainOnTrack.Train).ValueRW;
                var ds = dt * Math.Min(train.MaxSpeed, track.CurrentMaxSpeed);
                var dprog = ds / trackSpline.GetLength();
                trainsOnTrack.ElementAt(i).Progress += dprog;
                if (trainOnTrack.Progress < 1f)
                {
                    var newPos = trackSpline.EvaluatePosition(trainOnTrack.Progress);
                    train.Position = new float2(newPos.x, newPos.z);
                    UnityEngine.Debug.Log($"New pos {train.Position.x}/{train.Position.y}");
                }
                //TODO:отправить на станцию
            }
        }
        // private void ProcessTrain(Entity trainEntity, ref SystemState state, float dt)
        // {
        //     ref var train = ref SystemAPI.GetComponentRW<Train>(trainEntity).ValueRW;
        //     switch (train.State)
        //     {
        //         case TrainState.Idle:
        //         case TrainState.Stopped:
        //             return;
        //         case TrainState.Waiting:
        //             {
        //                 train.WaitTime -= dt;
        //                 if (train.WaitTime <= 0)
        //                     train.State = TrainState.OnWay;
        //                 return;
        //             }
        //         case TrainState.Destroyed:
        //             {
        //                 var visual = SystemAPI.GetComponentRO<TrainVisual>(trainEntity);
        //             }
        //     }

        //     var targetStation = SystemAPI.GetComponent<Station>(train.Target);
        //     if (train.Target == default) return;

        //     if ((train.Position.x - targetStation.Position.x) * (train.Position.x - targetStation.Position.x)
        //         + (train.Position.y - targetStation.Position.y) * (train.Position.y - targetStation.Position.y)
        //         > ContactDistance * ContactDistance)
        //     {
        //         if (train.Speed < train.MaxSpeed)
        //             train.Speed = Math.Min(train.Speed + 5f * dt, train.MaxSpeed);
        //         train.Position = CalculateNewPosition(train.Position, targetStation.Position, train.Speed * dt);
        //     }
        //     else
        //     {
        //         train.Speed = 0f;
        //         train.WaitTime = 3f;
        //         train.Position = targetStation.Position;
        //         train.Target = SelectNewTarget(train, ref state);
        //     }
        // }


        private float2 CalculateNewPosition(float2 from, float2 to, float ds)
        {
            var distance = (to.x - from.x) * (to.x - from.x) + (to.y - from.y) * (to.y - from.y);
            if (ds > distance)
                ds = distance;
            var newX = from.x + (to.x - from.x) * ds / distance;
            var newY = from.y + (to.y - from.y) * ds / distance;
            return new float2(newX, newY);
        }
    }
}