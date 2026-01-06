using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Application.Interfaces
{
    public interface IScene
    {
        string Name { get; }
        void Initialize(World world);
        void Cleanup(World world);
    }
}
