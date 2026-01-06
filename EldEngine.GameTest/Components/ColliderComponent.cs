using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    /// <summary>
    /// Componente de colisión (hitbox).
    /// </summary>
    public struct ColliderComponent : IComponent
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public bool IsSolid { get; set; }  // ¿Bloquea movimiento?
        public string Tag { get; set; }    // "player", "enemy", "wall", "item"

        public ColliderComponent(float width = 16, float height = 16, bool solid = false, string tag = "default")
        {
            Width = width;
            Height = height;
            IsSolid = solid;
            Tag = tag;
        }

        static int IComponent.GetComponentTypeId() => typeof(ColliderComponent).GetHashCode();
    }
}
