using EldEngine.Core.Domain.Entities;

namespace EldEngine.Core.Domain.Values
{
    /// <summary>
    /// Componente de velocidad en dos dimensiones. 
    /// </summary>
    
    public struct Velocity : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Velocity(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        static int IComponent.GetComponentTypeId() => typeof(Velocity).GetHashCode();
    }
}
