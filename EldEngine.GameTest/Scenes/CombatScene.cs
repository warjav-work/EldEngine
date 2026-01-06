using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Scenes
{
    /// <summary>
    /// Escena de combate 1v1 arena.
    /// </summary>
    public class CombatScene : IScene
    {
        private Entity _playerCombat;
        private Entity _enemyInCombat;

        public string Name => "CombatScene";

        public void Initialize(World world)
        {
            // Jugador en arena
            _playerCombat = world.CreateEntity();
            world.AddComponent(_playerCombat, new Transform(10, 10));
            world.AddComponent(_playerCombat, new CombatStats(100, 20, 5));

            // Enemigo en arena
            _enemyInCombat = world.CreateEntity();
            world.AddComponent(_enemyInCombat, new Transform(30, 10));
            world.AddComponent(_enemyInCombat, new CombatStats(50, 12, 3));

            System.Console.WriteLine("[CombatScene] Iniciando combate de arena");
        }

        public void Cleanup(World world)
        {
            world.DestroyEntity(_playerCombat);
            world.DestroyEntity(_enemyInCombat);
        }
    }
}
