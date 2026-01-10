using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Infrastructure.Events;

namespace EldEngine.GameTest.Events
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EnemyDefeatedEvent
    /// ═════════════════════════════════════════════════════════════════════════
    /// 
    /// Evento que se dispara cuando se derrota un enemigo.
    /// 
    /// FLUJO:
    /// 1. DeathSystem detecta que un enemigo tiene vida = 0
    /// 2. DeathSystem calcula puntos a recompensar
    /// 3. DeathSystem publica: _eventBus.Publish(new EnemyDefeatedEvent {...})
    /// 4. EventBus encola el evento
    /// 5. GameWindow.Paint() llama: _eventBus.ProcessEvents()
    /// 6. GameManager escucha y suma puntos
    /// 7. UIManager escucha y muestra mensaje flotante
    /// 8. StatsTracker escucha y actualiza estadísticas
    /// 9. AchievementSystem escucha y verifica logros
    /// 
    /// VENTAJA: Múltiples sistemas reaccionan sin acoplamiento
    ///
    public class EnemyDefeatedEvent : IGameEvent
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES DEL EVENTO
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// La entidad del enemigo derrotado.
        /// </summary>
        public Entity Enemy { get; set; }

        /// <summary>
        /// Puntos otorgados por derrotar este enemigo.
        /// Fórmula: BasePoints (50) + HealthBonus (MaxHealth * 2)
        /// 
        /// Ejemplo:
        /// - Goblin (20 HP): 50 + (20 * 2) = 90 puntos
        /// - Orc (40 HP): 50 + (40 * 2) = 130 puntos
        /// - Boss (100 HP): 50 + (100 * 2) = 250 puntos
        /// </summary>
        public int PointsRewarded { get; set; }

        /// <summary>
        /// Tipo de enemigo derrotado.
        /// Usado para estadísticas y achievements.
        /// Valores: "patrol", "follow", "aggressive", "boss"
        /// </summary>
        public string EnemyType { get; set; } = "unknown";

        /// <summary>
        /// Salud máxima que tenía el enemigo.
        /// Usado para calcular dificultad.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// Ataque que tenía el enemigo.
        /// Usado para achievements "Derrota enemigos fuertes"
        /// </summary>
        public int Attack { get; set; }

        /// <summary>
        /// Posición donde fue derrotado.
        /// Útil para efectos visuales de explosión.
        /// </summary>
        public (float x, float y) DefeatPosition { get; set; }

        /// <summary>
        /// Si fue un one-shot (derrota en un golpe).
        /// Usado para bonus especial.
        /// </summary>
        public bool WasOneShot { get; set; }

        /// <summary>
        /// Nombre del evento para debugging.
        /// </summary>
        public string EventName => nameof(EnemyDefeatedEvent);

        // ═══════════════════════════════════════════════════════════════════
        // CONSTRUCTORES
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public EnemyDefeatedEvent() { }

        /// <summary>
        /// Constructor esencial.
        /// </summary>
        public EnemyDefeatedEvent(Entity enemy, int pointsRewarded, string enemyType)
        {
            Enemy = enemy;
            PointsRewarded = pointsRewarded;
            EnemyType = enemyType;
        }

        /// <summary>
        /// Constructor completo con todos los datos.
        /// </summary>
        public EnemyDefeatedEvent(
            Entity enemy,
            int pointsRewarded,
            string enemyType,
            int maxHealth,
            int attack,
            (float x, float y) defeatPosition,
            bool wasOneShot = false)
        {
            Enemy = enemy;
            PointsRewarded = pointsRewarded;
            EnemyType = enemyType;
            MaxHealth = maxHealth;
            Attack = attack;
            DefeatPosition = defeatPosition;
            WasOneShot = wasOneShot;
        }

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Obtiene una descripción legible del evento.
        /// </summary>
        public override string ToString()
        {
            return $"EnemyDefeatedEvent[Enemy: {Enemy.Id}, " +
                   $"Type: {EnemyType}, " +
                   $"Points: {PointsRewarded}, " +
                   $"Health: {MaxHealth}, " +
                   $"Position: ({DefeatPosition.x:F0}, {DefeatPosition.y:F0}), " +
                   $"OneShot: {WasOneShot}]";
        }

        /// <summary>
        /// Obtiene el multiplicador de puntos según el tipo.
        /// </summary>
        public float GetDifficultyMultiplier()
        {
            return EnemyType switch
            {
                "patrol" => 1.0f,           // Normal
                "follow" => 1.5f,          // Más difícil
                "aggressive" => 2.0f,      // Muy difícil
                "boss" => 3.0f,            // Extremadamente difícil
                _ => 1.0f
            };
        }

        /// <summary>
        /// Calcula bonus si fue one-shot.
        /// </summary>
        public int GetOneShotBonus()
        {
            return WasOneShot ? PointsRewarded / 2 : 0;  // 50% de bonus
        }
    }
}
