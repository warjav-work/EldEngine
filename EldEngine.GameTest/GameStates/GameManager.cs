using EldEngine.Core.Application.Interfaces;

namespace EldEngine.GameTest.GameStates
{
    /// <summary>
    /// Gestor global del juego - Maneja estados, pausa, game over, etc.
    /// </summary>
    public class GameManager
    {
        private IGameService _gameService;
        private GameState _currentState = GameState.Menu;
        private GameState _previousState = GameState.Menu;
        private readonly Dictionary<string, object> _gameData = new();

        // Eventos
        public event Action<GameState> OnGameStateChanged;
        public event Action<string> OnGameMessage;

        // Propiedades públicas
        public GameState CurrentState => _currentState;
        public GameState PreviousState => _previousState;
        public bool IsPaused => _currentState == GameState.Paused;
        public bool IsGameOver => _currentState == GameState.GameOver;
        public bool IsPlaying => _currentState == GameState.Playing;
        public bool IsMenu => _currentState == GameState.Menu;

        // Datos del juego
        public int CurrentScore { get; set; } = 0;
        public int CurrentLevel { get; set; } = 1;
        public float PlayTime { get; private set; } = 0f;

        public void Initialize(IGameService gameService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            System.Diagnostics.Debug.WriteLine("✓ GameManager inicializado");
        }

        /// <summary>Cambia el estado del juego con validaciones</summary>
        public void SetGameState(GameState newState)
        {
            if (_currentState == newState)
                return;

            // No puedes ir de Paused a GameOver directamente
            if (_currentState == GameState.Paused && newState == GameState.GameOver)
            {
                SetGameState(GameState.Playing);
            }

            _previousState = _currentState;
            _currentState = newState;

            System.Diagnostics.Debug.WriteLine($"[STATE CHANGE] {_previousState} → {_currentState}");

            // Ejecutar lógica específica de estado
            ExecuteStateLogic(newState);

            // Notificar listeners
            OnGameStateChanged?.Invoke(_currentState);
        }

        /// <summary>
        /// Alterna entre Playing y Paused.
        /// </summary>
        public void TogglePause()
        {
            if (_currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
                OnGameMessage?.Invoke("⏸️ JUEGO PAUSADO");
            }
            else if (_currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
                OnGameMessage?.Invoke("▶️ JUEGO REANUDADO");
            }
        }

        /// <summary>Finaliza el juego con el estado GameOver</summary>
        public void GameOver(string reason = "")
        {
            SetGameState(GameState.GameOver);
            OnGameMessage?.Invoke($"💀 GAME OVER: {reason}");
        }

        /// <summary>Reinicia el juego</summary>
        public void Restart()
        {
            Reset();
            SetGameState(GameState.Playing);
            OnGameMessage?.Invoke("🔄 JUEGO REINICIADO");
        }

        /// <summary>Actualiza el tiempo de juego (llamar cada frame)</summary>
        public void Update(float deltaTime)
        {
            if (_currentState == GameState.Playing)
            {
                PlayTime += deltaTime;
            }
        }

        /// <summary>Suma puntos al score</summary>
        public void AddScore(int points)
        {
            if (_currentState == GameState.Playing)
            {
                CurrentScore += points;
                OnGameMessage?.Invoke($"⭐ +{points} puntos! Total: {CurrentScore}");
            }
        }

        /// <summary>Avanza al siguiente nivel</summary>
        public void NextLevel()
        {
            CurrentLevel++;
            CurrentScore += 100 * CurrentLevel; // Bonus por nivel
            OnGameMessage?.Invoke($"🎉 ¡NIVEL {CurrentLevel} DESBLOQUEADO!");
        }

        /// <summary>Guarda datos persisten del juego</summary>
        public void SaveData(string key, object value)
        {
            _gameData[key] = value;
            System.Diagnostics.Debug.WriteLine($"💾 Guardado: {key} = {value}");
        }

        /// <summary>
        /// Obtiene datos guardados.
        /// </summary>
        public T GetData<T>(string key)
        {
            return _gameData.TryGetValue(key, out var value)
                ? (T)value
                : throw new KeyNotFoundException($"Clave no encontrada: {key}");
        }

        /// <summary>
        /// Reinicia todos los datos y estado.
        /// </summary>
        public void Reset()
        {
            _gameData.Clear();
            _currentState = GameState.Menu;
            _previousState = GameState.Menu;
            CurrentScore = 0;
            CurrentLevel = 1;
            PlayTime = 0f;
            System.Diagnostics.Debug.WriteLine("🔄 GameManager reseteado");
        }

        // ==================== LÓGICA PRIVADA ====================

        private void ExecuteStateLogic(GameState newState)
        {
            switch (newState)
            {
                case GameState.Menu:
                    OnEnterMenu();
                    break;

                case GameState.Playing:
                    OnEnterPlaying();
                    break;

                case GameState.Paused:
                    OnEnterPaused();
                    break;

                case GameState.GameOver:
                    OnEnterGameOver();
                    break;
            }
        }

        private void OnEnterMenu()
        {
            System.Diagnostics.Debug.WriteLine("[MENU] Mostrando menú principal");
        }

        private void OnEnterPlaying()
        {
            System.Diagnostics.Debug.WriteLine("[PLAYING] Reanudando juego");
            // Reanudar física, audio, etc.
        }

        private void OnEnterPaused()
        {
            System.Diagnostics.Debug.WriteLine("[PAUSED] Pausando juego");
            // Pausar física, audio, etc.
        }

        private void OnEnterGameOver()
        {
            System.Diagnostics.Debug.WriteLine("[GAMEOVER] Mostrando pantalla de game over");
            SaveData("LastScore", CurrentScore);
            SaveData("LastLevel", CurrentLevel);
            SaveData("LastPlayTime", PlayTime);
        }

        public override string ToString()
        {
            return $@"
╔════════════════════════════════════╗
║        GAME STATUS                 ║
╠════════════════════════════════════╣
║ Estado: {_currentState,-28} ║
║ Puntos: {CurrentScore,-28} ║
║ Nivel: {CurrentLevel,-28} ║
║ Tiempo: {PlayTime:F1}s{new string(' ', 20)} ║
╚════════════════════════════════════╝";
        }
    }    
}
