using EldEngine.Core.Application.Interfaces;
using EldEngine.GameTest.GameStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.GameTest.Scenes
{
    /// <summary>Gestor de niveles - controla progresión y carga</summary>
    public class LevelManager
    {
        private IGameService _gameService;
        private ISceneService _sceneService;
        private GameManager _gameManager;

        private int _currentLevelIndex = 0;
        private List<string> _levelSequence = new();

        public int CurrentLevel => _currentLevelIndex + 1;
        public int TotalLevels => _levelSequence.Count;
        public string CurrentLevelName => _levelSequence[_currentLevelIndex];

        public event Action OnLevelCompleted;
        public event Action OnLevelFailed;

        public void Initialize(IGameService gameService, ISceneService sceneService, GameManager gameManager)
        {
            _gameService = gameService;
            _sceneService = sceneService;
            _gameManager = gameManager;

            // Registrar todas las escenas
            RegisterLevels();
        }

        private void RegisterLevels()
        {
            // Registrar escenas en orden
            _sceneService.RegisterScene("Level_01_Forest", new Level01_Forest());
            _sceneService.RegisterScene("Level_02_CaveEntrance", new Level02_CaveEntrance());
            _sceneService.RegisterScene("Level_03_DeepForest", new Level03_DeepForest());
            _sceneService.RegisterScene("Level_04_ElfVillage", new Level04_ElfVillage());
            _sceneService.RegisterScene("Level_05_AncientRuins", new Level05_AncientRuins());

            // Secuencia de niveles
            _levelSequence = new List<string>
            {
                "Level_01_Forest",
                "Level_02_CaveEntrance",
                "Level_03_DeepForest",
                "Level_04_ElfVillage",
                "Level_05_AncientRuins"
            };

            System.Diagnostics.Debug.WriteLine($"✓ {_levelSequence.Count} niveles registrados");
        }

        public void LoadLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= _levelSequence.Count)
            {
                System.Diagnostics.Debug.WriteLine("❌ Índice de nivel inválido");
                return;
            }

            _currentLevelIndex = levelIndex;
            var levelName = _levelSequence[_currentLevelIndex];

            System.Diagnostics.Debug.WriteLine($"[LEVEL] Cargando {levelName}...");
            _gameService.LoadScene(levelName);
            _gameManager.CurrentLevel = CurrentLevel;
        }

        public void LoadCurrentLevel()
        {
            LoadLevel(_currentLevelIndex);
        }

        public void NextLevel()
        {
            if (_currentLevelIndex < _levelSequence.Count - 1)
            {
                LoadLevel(_currentLevelIndex + 1);
                _gameManager.NextLevel();
                OnLevelCompleted?.Invoke();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("🎉 ¡GANASTE! Completaste todos los niveles");
                _gameManager.GameOver("¡VICTORIO! ¡Completaste el juego!");
            }
        }

        public void RestartLevel()
        {
            LoadLevel(_currentLevelIndex);
        }

        public bool IsLastLevel => _currentLevelIndex == _levelSequence.Count - 1;
    }
}
