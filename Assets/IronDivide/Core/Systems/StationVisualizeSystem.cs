using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using IronDivide.Core.Components;
using UnityEngine;

namespace IronDivide.Core.Systems
{
    [BurstCompile]
    public partial struct StationVisualizeSystem : ISystem
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
            foreach ((var station, var entity) in SystemAPI.Query<Station>().WithNone<StationVisual>().WithEntityAccess())
            {
                var stationObject = Object.Instantiate(prefab.Station);
                ecb.AddComponent<StationVisual>(entity, new StationVisual
                {
                    StationVisualObject = stationObject
                });
                stationObject.transform.position = new Vector3(station.Position.x, 0.05f, station.Position.y);
            }

            // foreach ((var train, var visual) in SystemAPI.Query<Train,TrainVisual>())
            // {
            //     visual.TrainVisualObject.transform.position = new Vector3(train.Position.x, 0.05f, train.Position.y);
            //     //visual.TrainVisualObject.transform.rotation = new Quaternion()
            // }
        }

        public void OnDestroy(ref SystemState state)
        {

        }
    }
}