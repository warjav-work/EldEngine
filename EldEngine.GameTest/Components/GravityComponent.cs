using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    public struct GravityComponent : IComponent
    {
        public float GravityScale { get; set; }
        public bool UseGravity { get; set; }

        public GravityComponent(float scale = 1f)
        {
            GravityScale = scale;
            UseGravity = true;
        }

        static int IComponent.GetComponentTypeId() => typeof(GravityComponent).GetHashCode();
    }
}
