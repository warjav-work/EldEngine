using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Application.Interfaces
{
    public interface ISceneService
    {
        void RegisterScene(string name, IScene scene);
        IScene GetScene(string name);
        void LoadScene(World world, string name);
        IEnumerable<string> GetSceneNames();
    }
}
