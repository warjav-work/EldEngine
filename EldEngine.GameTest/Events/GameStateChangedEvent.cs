using EldEngine.Core.Infrastructure.Events;

namespace EldEngine.GameTest.Events
{
    // ═════════════════════════════════════════════════════════════════════════
    // GameStateChangedEvent
    // ═════════════════════════════════════════════════════════════════════════
    /// 
    /// Evento que se dispara cuando cambia el estado del juego.
    /// 
    /// FLUJO:
    /// 1. GameManager detecta cambio de estado
    /// 2. GameManager publica: _eventBus.Publish(new GameStateChangedEvent {...})
    /// 3. Múltiples sistemas reaccionan al cambio:
    ///    - AudioManager: Cambia música
    ///    - UIManager: Actualiza HUD
    ///    - InputManager: Habilita/desabilita inputs
    ///    - PauseSystem: Congela/descongela física
    ///    - AnalyticsSystem: Registra evento
    ///
    public class GameStateChangedEvent : IGameEvent
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES DEL EVENTO
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Estado anterior (antes del cambio).
        /// Valores: "Menu", "Playing", "Paused", "GameOver"
        /// </summary>
        public string PreviousState { get; set; } = "";

        /// <summary>
        /// Estado nuevo (después del cambio).
        /// Valores: "Menu", "Playing", "Paused", "GameOver"
        /// </summary>
        public string NewState { get; set; } = "";

        /// <summary>
        /// Razón del cambio (opcional, para debugging).
        /// Ejemplos: "Usuario presionó ESC", "Jugador murió", "Nivel completado"
        /// </summary>
        public string Reason { get; set; } = "";

        /// <summary>
        /// Timestamp de cuándo ocurrió (para logs).
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Nombre del evento.
        /// </summary>
        public string EventName => nameof(GameStateChangedEvent);

        // ═══════════════════════════════════════════════════════════════════
        // CONSTRUCTORES
        // ═══════════════════════════════════════════════════════════════════

        public GameStateChangedEvent() { }

        public GameStateChangedEvent(string previousState, string newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }

        public GameStateChangedEvent(string previousState, string newState,
            string reason)
        {
            PreviousState = previousState;
            NewState = newState;
            Reason = reason;
            Timestamp = DateTime.Now;
        }

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS
        // ═══════════════════════════════════════════════════════════════════

        public override string ToString()
        {
            return $"GameStateChangedEvent[{PreviousState} → {NewState}, " +
                   $"Reason: {Reason}, Time: {Timestamp:HH:mm:ss}]";
        }

        /// <summary>
        /// ¿Es entrada a juego (Menu → Playing)?
        /// </summary>
        public bool IsGameStart()
        {
            return PreviousState == "Menu" && NewState == "Playing";
        }

        /// <summary>
        /// ¿Es pausa (Playing → Paused)?
        /// </summary>
        public bool IsPause()
        {
            return PreviousState == "Playing" && NewState == "Paused";
        }

        /// <summary>
        /// ¿Es reanuda (Paused → Playing)?
        /// </summary>
        public bool IsResume()
        {
            return PreviousState == "Paused" && NewState == "Playing";
        }

        /// <summary>
        /// ¿Es game over (Playing → GameOver)?
        /// </summary>
        public bool IsGameOver()
        {
            return PreviousState == "Playing" && NewState == "GameOver";
        }

        /// <summary>
        /// ¿Es vuelta a menú (cualquier → Menu)?
        /// </summary>
        public bool IsReturnToMenu()
        {
            return NewState == "Menu";
        }
    }
}
