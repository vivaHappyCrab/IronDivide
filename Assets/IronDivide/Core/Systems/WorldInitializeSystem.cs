using System;
using IronDivide.Core.Components;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine.Splines;
using Unity.Collections;

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
            var station1Buffer = ecb.AddBuffer<TrainElement>(station1);

            var station2 = ecb.CreateEntity();
            var station2Comp = new Station
            {
                Id = 2,
                Name = "Мак на 6ой радиальной",
                Position = new float2(0, 10)
            };
            ecb.AddComponent<Station>(station2, station2Comp);
            var station2Buffer = ecb.AddBuffer<TrainElement>(station2);

            var station3 = ecb.CreateEntity();
            var station3Comp = new Station
            {
                Id = 3,
                Name = "LIFE Варшавская",
                Position = new float2(0, 0)
            };
            ecb.AddComponent<Station>(station2, station2Comp);

            var stationBuffer = ecb.AddBuffer<StationElement>(worldStateEntity);
            stationBuffer.Add(new StationElement { Station = station1 });
            stationBuffer.Add(new StationElement { Station = station2 });
            stationBuffer.Add(new StationElement { Station = station3 });


            var track1 = ecb.CreateEntity();
            var track1Component = new Track
            {
                Id = 1,
                FromStation = station1,
                ToStation = station2,
                Bidirectional = true,
                MaxSpeed = 20f,
                CurrentMaxSpeed = 20f
            };
            ecb.AddComponent<Track>(track1, track1Component);
            ecb.AddComponent<SplineBlobAssetComponent>(track1,
                                                    CreateTrackBezier(station1Comp.Position, station2Comp.Position));
            var track1Buffer = ecb.AddBuffer<TrainOnTrackElement>(track1);

            var trackBuffer = ecb.AddBuffer<TrackElement>(worldStateEntity);
            trackBuffer.Add(new TrackElement { Track = track1 });



            var train1 = ecb.CreateEntity();
            var train1Component = new Train
            {
                Id = 1,
                Name = "901",
                Position = station1Comp.Position,
                MaxSpeed = 20f,
                State = TrainState.Idle
            };
            ecb.AddComponent<Train>(train1, train1Component);
            station1Buffer.Add(new TrainElement { Train = train1 });

            var train2 = ecb.CreateEntity();
            var train2Component = new Train
            {
                Id = 2,
                Name = "11k",
                Position = station1Comp.Position,
                MaxSpeed = 1f,
                State = TrainState.OnWay
            };
            ecb.AddComponent<Train>(train2, train2Component);
            track1Buffer.Add(new TrainOnTrackElement { Train = train2, Progress = 0f });

            var trainBuffer = ecb.AddBuffer<TrainElement>(worldStateEntity);
            trainBuffer.Add(new TrainElement { Train = train1 });
            trainBuffer.Add(new TrainElement { Train = train2 });

            state.Enabled = false;
        }

        public void OnDestroy(ref SystemState state)
        {

        }

        private SplineBlobAssetComponent CreateTrackBezier(float2 from, float2 to)
        {
            var knotFrom = new BezierKnot(new float3(from.x, 0, from.y));
            var knotTo = new BezierKnot(new float3(to.x, 0, to.y));
            var knotMiddle = new BezierKnot(new float3((from.x + to.x) / 2 + 3, 0, (from.y + to.y) / 2 + 4));
            using var nativeList = new NativeList<BezierKnot>(initialCapacity: 3, Allocator.Temp);
            nativeList.Add(knotFrom);
            nativeList.Add(knotMiddle);
            nativeList.Add(knotTo);

            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<NativeSplineBlob>();
            root.transformMatrix = float4x4.identity;
            root.closed = false;
            var array = builder.Allocate(ref root.knots, nativeList.Length);
            for (int i = 0; i < nativeList.Length; ++i)
            {
                array[i] = nativeList[i];
            }
            var blobAssetRef = builder.CreateBlobAssetReference<NativeSplineBlob>(Allocator.Persistent);
            return new SplineBlobAssetComponent { reference = blobAssetRef };
        }


    }
}