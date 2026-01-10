using IronDivide.Core.Components;
using Unity.Entities;
using UnityEngine;

namespace IronDivide.Authorings
{

    class TrainAuthoring : MonoBehaviour
    {

    }

    class TrainAuthoringBaker : Baker<TrainAuthoring>
    {
        public override void Bake(TrainAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Train());
        }
    }
}
