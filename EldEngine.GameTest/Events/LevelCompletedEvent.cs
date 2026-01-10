using EldEngine.Core.Infrastructure.Events;

namespace EldEngine.GameTest.Events
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// LevelCompletedEvent
    /// ═════════════════════════════════════════════════════════════════════════
    /// 
    /// Evento que se dispara cuando se completa un nivel.
    /// 
    /// FLUJO:
    /// 1. LevelManager detecta: todos enemigos derrotados + objetivos completados
    /// 2. LevelManager publica: _eventBus.Publish(new LevelCompletedEvent {...})
    /// 3. GameManager escucha y llama: _gameManager.NextLevel()
    /// 4. UIManager escucha y muestra pantalla de felicitaciones
    /// 5. AudioManager escucha y reproduce música de victoria
    /// 6. StatsTracker escucha y guarda tiempo para leaderboard
    ///
    public class LevelCompletedEvent : IGameEvent
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES DEL EVENTO
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Número del nivel completado (1-5).
        /// </summary>
        public int LevelNumber { get; set; }

        /// <summary>
        /// Nombre del nivel para mostrar al usuario.
        /// Ejemplos: "Bosque Élfico", "Entrada a Cueva", "Ruinas Antiguas"
        /// </summary>
        public string LevelName { get; set; } = "";

        /// <summary>
        /// Puntos totales conseguidos en el nivel.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Tiempo que tardó en completar el nivel.
        /// </summary>
        public float TimeSpent { get; set; }

        /// <summary>
        /// Número de enemigos derrotados.
        /// </summary>
        public int EnemiesDefeated { get; set; }

        /// <summary>
        /// Número de items recogidos.
        /// </summary>
        public int ItemsCollected { get; set; }

        /// <summary>
        /// Rating del nivel (1-5 estrellas basado en desempeño).
        /// 5 = Sin daño, rápido
        /// 4 = Poco daño, rápido
        /// 3 = Daño moderado
        /// 2 = Mucho daño
        /// 1 = Completado
        /// </summary>
        public int StarRating { get; set; }

        /// <summary>
        /// Bonus de puntos por rapidez.
        /// Cuanto más rápido, más bonus.
        /// </summary>
        public int SpeedBonus { get; set; }

        /// <summary>
        /// Es el último nivel del juego?
        /// </summary>
        public bool IsLastLevel { get; set; }

        /// <summary>
        /// Nombre del evento.
        /// </summary>
        public string EventName => nameof(LevelCompletedEvent);

        // ═══════════════════════════════════════════════════════════════════
        // CONSTRUCTORES
        // ═══════════════════════════════════════════════════════════════════

        public LevelCompletedEvent() { }

        public LevelCompletedEvent(int levelNumber, string levelName,
            int score, float timeSpent)
        {
            LevelNumber = levelNumber;
            LevelName = levelName;
            Score = score;
            TimeSpent = timeSpent;
        }

        public LevelCompletedEvent(int levelNumber, string levelName,
            int score, float timeSpent, int enemiesDefeated,
            int itemsCollected, int starRating, int speedBonus, bool isLastLevel)
        {
            LevelNumber = levelNumber;
            LevelName = levelName;
            Score = score;
            TimeSpent = timeSpent;
            EnemiesDefeated = enemiesDefeated;
            ItemsCollected = itemsCollected;
            StarRating = starRating;
            SpeedBonus = speedBonus;
            IsLastLevel = isLastLevel;
        }

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS
        // ═══════════════════════════════════════════════════════════════════

        public override string ToString()
        {
            return $"LevelCompletedEvent[Level: {LevelNumber}, " +
                   $"Name: {LevelName}, " +
                   $"Score: {Score}, " +
                   $"Time: {TimeSpent:F1}s, " +
                   $"Enemies: {EnemiesDefeated}, " +
                   $"Stars: {StarRating}/5]";
        }

        /// <summary>
        /// Obtiene los emojis de estrellas para mostrar.
        /// </summary>
        public string GetStarEmojis()
        {
            return new string('⭐', StarRating) +
                   new string('☆', 5 - StarRating);
        }

        /// <summary>
        /// Obtiene descripción del desempeño.
        /// </summary>
        public string GetPerformanceDescription()
        {
            return StarRating switch
            {
                5 => "¡Perfecto! ¡Sin un solo rasguño!",
                4 => "¡Excelente! Muy buen trabajo",
                3 => "Bien hecho. Puedes hacerlo mejor",
                2 => "Completado. Sigue practicando",
                _ => "¡Lo lograste!"
            };
        }
    }
}
