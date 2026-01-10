using Unity.Entities;

namespace IronDivide.Core.Components
{
    public struct Grid : IComponentData
    {
        public float cellWidth;

        public float cellHeight;
    }
}