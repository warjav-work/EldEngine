using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Application.Interfaces
{
    /// <summary>
    /// Servicio principal que desacopla la interfaz del motor ECS.
    /// </summary>
    public interface IGameService : IDisposable
    {
        World World { get; }

        void Initialize();
        void Update(float deltaTime);
        void LoadScene(string sceneName);
        void SaveScene(string scenePath);

        event Action<string> OnError;
        event Action OnInitialized;
    }
}
