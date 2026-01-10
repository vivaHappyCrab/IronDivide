using UnityEngine;
using Unity.Entities;
using IronDivide.Core.Components;

namespace IronDivide.Authorings
{
    public class WorldStateAuthoring : MonoBehaviour
    {
        public int Seed;
    }

    public class WorldStateBaker : Baker<WorldStateAuthoring>
    {
        public override void Bake(WorldStateAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new WorldState
            {
                Seed = authoring.Seed
            });
        }
    }
}