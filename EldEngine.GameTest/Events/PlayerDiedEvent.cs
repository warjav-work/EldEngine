using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Infrastructure.Events;

namespace EldEngine.GameTest.Events
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// PlayerDiedEvent
    /// ═════════════════════════════════════════════════════════════════════════
    /// 
    /// Evento que se dispara cuando el jugador muere.
    /// 
    /// FLUJO:
    /// 1. DeathSystem detecta que el jugador tiene vida = 0
    /// 2. DeathSystem publica: _eventBus.Publish(new PlayerDiedEvent {...})
    /// 3. EventBus encola el evento
    /// 4. GameWindow.Paint() llama: _eventBus.ProcessEvents()
    /// 5. GameManager escucha y reacciona: _gameManager.GameOver()
    /// 
    /// VENTAJA: DeathSystem NO conoce GameManager
    ///
    public class PlayerDiedEvent : IGameEvent
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES DEL EVENTO
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// La entidad del jugador que murió.
        /// </summary>
        public Entity Player { get; set; }

        /// <summary>
        /// Razón por la que murió (para mostrar al usuario).
        /// Ejemplos:
        /// - "El jugador fue derrotado en combate"
        /// - "Aplastado por un obstáculo"
        /// - "Envenenado"
        /// - "Quemado"
        /// </summary>
        public string Reason { get; set; } = "Razón desconocida";

        /// <summary>
        /// Quién mató al jugador (si aplica).
        /// Útil para achievements: "Derrotado por un Boss"
        /// </summary>
        public Entity? Killer { get; set; }

        /// <summary>
        /// Daño final que recibió.
        /// </summary>
        public int FinalDamage { get; set; }

        /// <summary>
        /// Vida máxima que tenía.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// Timestamp de cuándo murió (para estadísticas).
        /// </summary>
        public float TimeOfDeath { get; set; }

        /// <summary>
        /// Nombre del evento para debugging.
        /// </summary>
        public string EventName => nameof(PlayerDiedEvent);

        // ═══════════════════════════════════════════════════════════════════
        // CONSTRUCTORES
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public PlayerDiedEvent() { }

        /// <summary>
        /// Constructor con datos esenciales.
        /// </summary>
        public PlayerDiedEvent(Entity player, string reason)
        {
            Player = player;
            Reason = reason;
        }

        /// <summary>
        /// Constructor completo.
        /// </summary>
        public PlayerDiedEvent(Entity player, string reason, Entity? killer,
            int finalDamage, int maxHealth, float timeOfDeath)
        {
            Player = player;
            Reason = reason;
            Killer = killer;
            FinalDamage = finalDamage;
            MaxHealth = maxHealth;
            TimeOfDeath = timeOfDeath;
        }

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Obtiene una descripción legible del evento.
        /// </summary>
        public override string ToString()
        {
            return $"PlayerDiedEvent[Player: {Player.Id}, " +
                   $"Reason: {Reason}, " +
                   $"Killer: {(Killer.HasValue ? Killer.Value.Id : "N/A")}, " +
                   $"Damage: {FinalDamage}, " +
                   $"Time: {TimeOfDeath:F1}s]";
        }
    }
}
