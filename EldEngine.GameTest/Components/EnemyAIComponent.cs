using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    /// <summary>
    /// Comportamiento de IA para enemigos.
    /// </summary>
    public struct EnemyAIComponent : IComponent
    {
        public string AIType { get; set; }           // "patrol", "follow", "aggressive"
        public float DetectionRange { get; set; }    // Rango para detectar jugador
        public float AttackRange { get; set; }       // Rango para atacar
        public float PatrolSpeed { get; set; }       // Velocidad en patrulla
        public float ChaseSpeed { get; set; }        // Velocidad al perseguir
        public float StateChangeTime { get; set; }   // Tiempo para cambiar estado
        public bool CanSeePlayer { get; set; }       // ¿Ve al jugador?

        public EnemyAIComponent(
            string aiType = "patrol",
            float detectionRange = 150f,
            float attackRange = 20f,
            float patrolSpeed = 2f,
            float chaseSpeed = 4f)
        {
            AIType = aiType;
            DetectionRange = detectionRange;
            AttackRange = attackRange;
            PatrolSpeed = patrolSpeed;
            ChaseSpeed = chaseSpeed;
            StateChangeTime = 0;
            CanSeePlayer = false;
        }

        static int IComponent.GetComponentTypeId() => typeof(EnemyAIComponent).GetHashCode();
    }
}
