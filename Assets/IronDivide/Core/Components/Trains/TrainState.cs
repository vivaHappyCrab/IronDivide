using Unity.Entities;

namespace IronDivide.Core.Components
{
    public enum TrainState
    {
        Idle,
        OnWay,
        Waiting,
        Stopped,
        Captured,
        Destroyed
    }
}