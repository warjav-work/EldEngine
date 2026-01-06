using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Worlds;
using EldEngine.Core.Infrastructure.Serialization;

namespace EldEngine.Core.Application.Services
{
    /// <summary>
    /// Servicio principal que coordina el motor y los sistemas.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly World _world;
        private readonly ISceneService _sceneService;
        private readonly IInputService _inputService;
        private readonly SceneSerializer _serializer;
        private bool _initialized = false;

        public World World => _world;

        public event Action<string> OnError;
        public event Action OnInitialized;

        public GameService(ISceneService sceneService, IInputService inputService)
        {
            _world = new World();
            _sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
            _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
            _serializer = new SceneSerializer();
        }

        public void Initialize()
        {
            try
            {
                RegisterDefaultSystems();
                _initialized = true;
                OnInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Error inicializando GameService: {ex.Message}");
                throw;
            }
        }

        public void Update(float deltaTime)
        {
            if (!_initialized)
                throw new InvalidOperationException("GameService no inicializado");

            _world.Update(deltaTime);
        }

        public void LoadScene(string sceneName)
        {
            try
            {
                var scene = _sceneService.GetScene(sceneName);
                scene?.Initialize(_world);
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Error cargando escena '{sceneName}': {ex.Message}");
            }
        }

        public void SaveScene(string scenePath)
        {
            try
            {
                _serializer.SerializeScene(_world, scenePath);
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Error guardando escena: {ex.Message}");
            }
        }

        private void RegisterDefaultSystems()
        {
            // Los sistemas se registran aquí. Ejemplo:
            _world.AddSystem(new MovementSystem());
        }

        public void Dispose()
        {
            _world?.Dispose();
        }
    }
}
