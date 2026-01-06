using EldEngine.Core.Domain.Entities;

namespace EldEngine.Core.Domain.Values
{
    /// <summary>
    /// Componente de transformación para posición y rotación.
    /// </summary>
    public struct Transform : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        public Transform(float x = 0, float y = 0, float rotation = 0)
        {
            X = x;
            Y = y;
            Rotation = rotation;
            ScaleX = 1f;
            ScaleY = 1f;
        }

        static int IComponent.GetComponentTypeId() => typeof(Transform).GetHashCode();

        public override string ToString() => $"Transform({X:F1}, {Y:F1}, {Rotation:F1}°)";
    }
}
