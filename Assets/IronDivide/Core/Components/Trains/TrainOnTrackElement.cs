using Unity.Entities;

namespace IronDivide.Core.Components
{
    public struct TrainOnTrackElement : IBufferElementData
    {
        public Entity Train;

        public float Progress;
    }
}