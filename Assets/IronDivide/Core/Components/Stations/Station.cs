using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace IronDivide.Core.Components
{
    public struct Station : IComponentData
    {
        public ulong Id;
        public FixedString128Bytes Name;

        public float2 Position;
        public DynamicBuffer<TrainElement> Trains;
    }
}