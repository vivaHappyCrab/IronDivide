using Unity.Collections;
using Unity.Entities;

namespace IronDivide.Core.Components
{
    public struct Order : IComponentData
    {
        public ulong Id;

        public FixedString128Bytes Name;

        public OrderContent Content;

        public Entity TargetStation;

        public float Deadline;
    }
}