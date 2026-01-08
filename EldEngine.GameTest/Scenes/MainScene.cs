using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Scenes
{
    /// <summary>
    /// Escena principal: Bosque élfico con NPCs y enemigos.
    /// </summary>
    public class MainScene : IScene
    {
        public string Name => "MainScene";

        public void Initialize(World world)
        {
            // Crear jugador
            var player = world.CreateEntity();
            world.AddComponent(player, new Transform(10, 10));
            world.AddComponent(player, new Velocity());
            world.AddComponent(player, new PlayerController(moveSpeed: 15f));
            world.AddComponent(player, new CombatStats(maxHp: 100, attack: 15, defense: 5));
            world.AddComponent(player, new InventoryComponent { MaxSlots = 20 });
            //world.AddComponent(player, new GravityComponent(1f));

            // Crear aldeano amigable
            var villager = world.CreateEntity();
            world.AddComponent(villager, new Transform(20, 15));
            world.AddComponent(villager, new NpcBehavior("Arandor el Sabio", "dialogue.villager_intro"));

            // Crear enemigos (goblins)
            for (int i = 0; i < 3; i++)
            {
                var goblin = world.CreateEntity();
                world.AddComponent(goblin, new Transform(30 + i * 5, 20));
                world.AddComponent(goblin, new Velocity());
                world.AddComponent(goblin, new CombatStats(maxHp: 20, attack: 8, defense: 1));
            }

            // Crear objetos del escenario
            var tree = world.CreateEntity();
            world.AddComponent(tree, new Transform(5, 5));
            // Añadir componente visual si lo hubiera

            System.Console.WriteLine($"[{Name}] Escena cargada con {3 + 1 + 1} entidades");
        }

        public void Cleanup(World world)
        {
            System.Console.WriteLine($"[{Name}] Limpiando escena");
        }
    }
}
