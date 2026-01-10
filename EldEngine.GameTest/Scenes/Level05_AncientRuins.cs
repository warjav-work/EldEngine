using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Scenes
{
    public class Level05_AncientRuins : IScene
    {
        public string Name => "Level_05_AncientRuins";

        public void Initialize(World world)
        {
            System.Diagnostics.Debug.WriteLine("\n╔════════════════════════════════════╗");
            System.Diagnostics.Debug.WriteLine("║  NIVEL 5: RUINAS ANTIGUAS (BOSS)   ║");
            System.Diagnostics.Debug.WriteLine("║  ¡Derrota al Señor de Sombra!      ║");
            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════╝\n");

            var player = world.CreateEntity();
            world.AddComponent(player, new Transform(50, 400));
            world.AddComponent(player, new Velocity());
            world.AddComponent(player, new PlayerController(15f));
            world.AddComponent(player, new CombatStats(150, 25, 8));
            world.AddComponent(player, new InventoryComponent());
            world.AddComponent(player, new ColliderComponent(16, 16, true, "player"));

            // NPCs finales
            CreateNPC(world, 150, 100, "Eldor", "El Anciano Sabio");

            // Boss final con estadísticas aumentadas
            CreateBoss(world, 400, 200);

            // Guardias del boss
            CreateEnemy(world, 350, 150, "aggressive", 40, 16, 2.5f);
            CreateEnemy(world, 450, 150, "aggressive", 40, 16, 2.5f);
            CreateEnemy(world, 300, 300, "follow", 35, 14, 2.3f);
            CreateEnemy(world, 500, 300, "follow", 35, 14, 2.3f);

            // Más obstáculos (ruinas)
            for (int i = 0; i < 12; i++)
            {
                CreateObstacle(world, 100 + i * 40, 250 + (i % 3) * 60, "rock");
            }

            CreateWalls(world, 600, 500);

            System.Diagnostics.Debug.WriteLine("✓ Nivel 5 cargado (¡Boss y guardias!)");
        }

        public void Cleanup(World world) { }

        private void CreateNPC(World world, float x, float y, string name, string title)
        {
            var npc = world.CreateEntity();
            world.AddComponent(npc, new Transform(x, y));
            world.AddComponent(npc, new NpcBehavior(name, $"dialogue.{name.ToLower()}"));
            world.AddComponent(npc, new ColliderComponent(14, 14, false, "npc"));
        }

        /// <summary>
        /// Crea el boss final - Mucho más fuerte que enemigos normales.
        /// </summary>
        private void CreateBoss(World world, float x, float y)
        {
            var boss = world.CreateEntity();
            world.AddComponent(boss, new Transform(x, y));
            world.AddComponent(boss, new Velocity());

            // Stats aumentados: 100 HP, 30 ATK, 10 DEF
            world.AddComponent(boss, new CombatStats(100, 30, 10));

            // IA agresiva con rango extendido
            var bossAI = new EnemyAIComponent("aggressive", 180f, 30f, 2f, 3f);
            world.AddComponent(boss, bossAI);

            world.AddComponent(boss, new ColliderComponent(20, 20, true, "boss"));

            System.Diagnostics.Debug.WriteLine("[BOSS] ¡¡¡ BOSS FINAL CREADO !!! 💀");
        }

        private void CreateEnemy(World world, float x, float y, string aiType, int health, int attack, float speed)
        {
            var enemy = world.CreateEntity();
            world.AddComponent(enemy, new Transform(x, y));
            world.AddComponent(enemy, new Velocity());
            world.AddComponent(enemy, new CombatStats(health, attack, 4));
            world.AddComponent(enemy, new EnemyAIComponent(aiType, 150f, 25f, speed, speed * 1.5f));
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