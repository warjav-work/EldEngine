using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.Core.Infrastructure.Events;
using EldEngine.GameTest.Components;
using EldEngine.GameTest.Events;
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

        private EventBus _eventBus;

        private int _currentLevelIndex = 0;
        private List<string> _levelSequence = new();
        private IScene _currentScene = null;

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

            _eventBus = new EventBus();

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
            // Limpiar escena anterior
            if (_currentScene != null)
            {
                System.Diagnostics.Debug.WriteLine($"🧹 Limpiando escena anterior...");
                _currentScene.Cleanup(_gameService.World);
            }

            // Limpiar todas las entidades del mundo
            ClearAllEntities(_gameService.World);

            // Cargar nueva escena
            _currentLevelIndex = levelIndex;
            var levelName = _levelSequence[_currentLevelIndex];

            System.Diagnostics.Debug.WriteLine($"\n[LEVEL] Cargando {levelName}...");
            _currentScene = _sceneService.GetScene(levelName);
            _currentScene.Initialize(_gameService.World);

            _gameManager.CurrentLevel = CurrentLevel;

            System.Diagnostics.Debug.WriteLine($"✓ Nivel {CurrentLevel} cargado\n");
        }

        private void ClearAllEntities(World world)
        {
            //world.ClearAll();
            
            // Obtener todas las entidades antes de limpiar
            var allEntities = world.GetEntitiesWith<Transform>().ToList();

            System.Diagnostics.Debug.WriteLine($"🧹 Destruyendo {allEntities.Count} entidades...");

            // Destruir cada una
            foreach (var entity in allEntities)
            {
                if (entity.IsValid)
                {
                    world.DestroyEntity(entity);
                }
            }

            System.Diagnostics.Debug.WriteLine($"✓ Todas las entidades destruidas");
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

        public void CheckLevelCompletion()
        {
            // Verificar si todos los enemigos están derrotados
            var remainingEnemies = _gameService.World
                .GetEntitiesWith<EnemyAIComponent>()
                .Count();

            if (remainingEnemies == 0)
            {
                // ✅ Nivel completado
                var evt = new LevelCompletedEvent(
                    levelNumber: _currentLevelIndex + 1,
                    levelName: "Bosque Élfico",
                    score: _gameManager.CurrentScore,
                    timeSpent: _gameManager.PlayTime,
                    enemiesDefeated: 10,
                    itemsCollected: 5,
                    starRating: CalculateStarRating(),
                    speedBonus: CalculateSpeedBonus(),
                    isLastLevel: _currentLevelIndex >= _levelSequence.Count - 1
                );

                _eventBus.Publish(evt);
            }
        }

        private int CalculateStarRating()
        {
            // Lógica para calcular estrellas
            // 5 estrellas si: < 60s, sin daño
            // 4 estrellas si: < 90s, poco daño
            // etc.
            if (_gameManager.PlayTime < 60)
                return 5;
            else if (_gameManager.PlayTime < 90)
                return 4;
            else if (_gameManager.PlayTime < 120)
                return 3;
            else
                return 2;
        }

        private int CalculateSpeedBonus()
        {
            // Bonus: 1 punto por segundo ahorrado (máximo 100)
            if (_gameManager.PlayTime < 60)
                return 100;
            return Math.Max(0, (int)(120 - _gameManager.PlayTime));
        }

        public bool IsLastLevel => _currentLevelIndex == _levelSequence.Count - 1;
    }
}
