using EldEngine.Core.Domain.Entities;

namespace EldEngine.Core.Domain.Components
{
    public struct TransformComponent : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        public TransformComponent(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
            Rotation = 0;
            ScaleX = 1f;
            ScaleY = 1f;
        }

        static int IComponent.GetComponentTypeId() => typeof(TransformComponent).GetHashCode();

        public override string ToString() =>
            $"Transform({X:F1}, {Y:F1}, {Rotation:F1}°)";
    }
}
