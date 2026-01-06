using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Application.Services
{
    public class SceneService : ISceneService
    {
        private readonly Dictionary<string, IScene> _scenes = new();

        public void RegisterScene(string name, IScene scene)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Nombre de escena no puede estar vacío", nameof(name));

            _scenes[name] = scene ?? throw new ArgumentNullException(nameof(scene));
        }

        public IScene GetScene(string name) =>
            _scenes.TryGetValue(name, out var scene)
                ? scene
                : throw new KeyNotFoundException($"Escena '{name}' no registrada");

        public void LoadScene(World world, string name) =>
            GetScene(name)?.Initialize(world);

        public IEnumerable<string> GetSceneNames() => _scenes.Keys;
    }
}
