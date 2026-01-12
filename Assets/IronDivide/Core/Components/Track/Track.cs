using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace IronDivide.Core.Components
{
    public struct Track : IComponentData
    {
        public ulong Id;
        
        public Entity FromStation;
        
        public Entity ToStation;

        public bool Bidirectional;

        public float MaxSpeed;

        public float CurrentMaxSpeed;

        public float Length;
    }
}