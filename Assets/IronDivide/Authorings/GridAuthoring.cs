using UnityEngine;
using Unity.Entities;

namespace IronDivide.Authorings
{
    public class GridAuthoring : MonoBehaviour
    {
        public Grid Grid;
    }

    public class GridBaker : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Core.Components.Grid{
                cellWidth = authoring.Grid.cellSize.x,
                cellHeight = authoring.Grid.cellSize.y,                
                });
        }
    }
}