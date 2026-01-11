using UnityEngine;
using Unity.Entities;
using IronDivide.Core.Components;

namespace IronDivide.Authorings
{
    public class PrefabsAuthoring : MonoBehaviour
    {
        public GameObject Train;
    }

    public class PrefabsAuthoringBaker : Baker<PrefabsAuthoring>
    {
        public override void Bake(PrefabsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponentObject<PrefabStorage>(entity, new PrefabStorage
            {
                Train = authoring.Train
            });
        }
    }
}