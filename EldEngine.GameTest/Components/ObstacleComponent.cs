using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    /// <summary>
    /// Componente para obstáculos (paredes, árboles).
    /// </summary>
    public struct ObstacleComponent : IComponent
    {
        public string ObstacleType { get; set; }  // "wall", "tree", "rock", "door"
        public bool CanBreak { get; set; }        // ¿Puede destruirse?
        public int Health { get; set; }           // Salud del obstáculo
        public int MaxHealth { get; set; }

        public ObstacleComponent(string type = "wall", bool canBreak = false, int health = 100)
        {
            ObstacleType = type;
            CanBreak = canBreak;
            Health = health;
            MaxHealth = health;
        }

        static int IComponent.GetComponentTypeId() => typeof(ObstacleComponent).GetHashCode();
    }
}
