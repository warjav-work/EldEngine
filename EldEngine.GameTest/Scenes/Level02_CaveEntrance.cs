using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Scenes
{
    /// <summary>Nivel 2: Entrada a Cueva - Dificultad media</summary>
    public class Level02_CaveEntrance : IScene
    {
        public string Name => "Level_02_CaveEntrance";

        public void Initialize(World world)
        {
            System.Diagnostics.Debug.WriteLine("\n╔════════════════════════════════════╗");
            System.Diagnostics.Debug.WriteLine("║  NIVEL 2: ENTRADA A CUEVA          ║");
            System.Diagnostics.Debug.WriteLine("║  Derrota 5 orcos - Evita rocas     ║");
            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════╝\n");

            var player = world.CreateEntity();
            world.AddComponent(player, new Transform(50, 400));
            world.AddComponent(player, new Velocity());
            world.AddComponent(player, new PlayerController(15f));
            world.AddComponent(player, new CombatStats(100, 15, 5));
            world.AddComponent(player, new InventoryComponent());
            world.AddComponent(player, new ColliderComponent(16, 16, true, "player"));

            // NPCs
            CreateNPC(world, 150, 100, "Theron", "Guardian");

            // Enemigos más fuertes (Orcos - Agresivos)
            CreateEnemy(world, 300, 200, "aggressive", 30, 12, 2.5f);
            CreateEnemy(world, 400, 250, "aggressive", 30, 12, 2.5f);
            CreateEnemy(world, 350, 400, "follow", 25, 10, 2f);
            CreateEnemy(world, 450, 350, "aggressive", 30, 12, 2.5f);
            CreateEnemy(world, 300, 450, "follow", 25, 10, 2f);

            // Más obstáculos (rocas, estalactitas)
            for (int i = 0; i < 8; i++)
            {
                CreateObstacle(world, 150 + i * 50, 250 + (i % 2) * 50, "rock");
            }

            CreateWalls(world, 600, 500);

            System.Diagnostics.Debug.WriteLine("✓ Nivel 2 cargado");
        }

        public void Cleanup(World world) { }

        private void CreateNPC(World world, float x, float y, string name, string title)
        {
            var npc = world.CreateEntity();
            world.AddComponent(npc, new Transform(x, y));
            world.AddComponent(npc, new NpcBehavior(name, $"dialogue.{name.ToLower()}"));
            world.AddComponent(npc, new ColliderComponent(14, 14, false, "npc"));
        }

        private void CreateEnemy(World world, float x, float y, string aiType, int health, int attack, float speed)
        {
            var enemy = world.CreateEntity();
            world.AddComponent(enemy, new Transform(x, y));
            world.AddComponent(enemy, new Velocity());
            world.AddComponent(enemy, new CombatStats(health, attack, 3));
            world.AddComponent(enemy, new EnemyAIComponent(aiType, 130f, 25f, speed, speed * 1.5f));
            world.AddComponent(enemy, new ColliderComponent(16, 16, true, "enemy"));
        }

        private void CreateObstacle(World world, float x, float y, string type)
        {
            var obstacle = world.CreateEntity();
            world.AddComponent(obstacle, new Transform(x, y));
            world.AddComponent(obstacle, new ObstacleComponent(type, false, 150));
            world.AddComponent(obstacle, new ColliderComponent(18, 18, true, type));
        }

        private void CreateWalls(World world, float mapWidth, float mapHeight)
        {
            var wallTop = world.CreateEntity();
            world.AddComponent(wallTop, new Transform(mapWidth / 2, -10));
            world.AddComponent(wallTop, new ColliderComponent(mapWidth, 20, true, "wall"));

            var wallBottom = world.CreateEntity();
            world.AddComponent(wallBottom, new Transform(mapWidth / 2, mapHeight + 10));
            world.AddComponent(wallBottom, new ColliderComponent(mapWidth, 20, true, "wall"));

            var wallLeft = world.CreateEntity();
            world.AddComponent(wallLeft, new Transform(-10, mapHeight / 2));
            world.AddComponent(wallLeft, new ColliderComponent(20, mapHeight, true, "wall"));

            var wallRight = world.CreateEntity();
            world.AddComponent(wallRight, new Transform(mapWidth + 10, mapHeight / 2));
            world.AddComponent(wallRight, new ColliderComponent(20, mapHeight, true, "wall"));
        }
    }
}
