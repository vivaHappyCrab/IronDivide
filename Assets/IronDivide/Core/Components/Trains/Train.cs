using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace IronDivide.Core.Components
{
    public struct Train : IComponentData
    {
        public ulong Id;

        public FixedString128Bytes Name;

        public float2 Position;

        public quaternion Rotation;

        public Entity Order;

        public float Speed;

        public float MaxSpeed;

        public float WaitTime;

        public TrainState State;
    }
}