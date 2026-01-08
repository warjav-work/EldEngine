using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Scenes
{
    public class Level03_DeepForest : IScene
    {
        public string Name => "Level_03_DeepForest";

        public void Initialize(World world)
        {
            System.Diagnostics.Debug.WriteLine("\n╔════════════════════════════════════╗");
            System.Diagnostics.Debug.WriteLine("║  NIVEL 3: BOSQUE PROFUNDO          ║");
            System.Diagnostics.Debug.WriteLine("║  Maze de árboles - 6 enemigos      ║");
            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════╝\n");

            var player = world.CreateEntity();
            world.AddComponent(player, new Transform(50, 400));
            world.AddComponent(player, new Velocity());
            world.AddComponent(player, new PlayerController(15f));
            world.AddComponent(player, new CombatStats(120, 18, 6));
            world.AddComponent(player, new InventoryComponent());
            world.AddComponent(player, new ColliderComponent(16, 16, true, "player"));

            // Enemigos en patrón
            CreateEnemy(world, 250, 150, "patrol", 25, 10, 2f);
            CreateEnemy(world, 350, 200, "follow", 25, 10, 2.2f);
            CreateEnemy(world, 400, 350, "aggressive", 35, 14, 2.5f);
            CreateEnemy(world, 300, 400, "follow", 25, 10, 2.2f);
            CreateEnemy(world, 450, 250, "patrol", 25, 10, 2f);
            CreateEnemy(world, 280, 300, "aggressive", 35, 14, 2.5f);

            // Laberinto de árboles
            CreateForestMaze(world);

            CreateWalls(world, 600, 500);

            System.Diagnostics.Debug.WriteLine("✓ Nivel 3 cargado");
        }

        public void Cleanup(World world) { }

        private void CreateForestMaze(World world)
        {
            // Crear patrón de laberinto
            float[][] mazePattern = new float[][]
            {
                new float[] { 100, 250, 200, 250, 300, 250 },
                new float[] { 150, 150, 250, 150, 350, 150 },
                new float[] { 200, 350, 300, 350, 400, 350 },
                new float[] { 100, 450, 200, 450, 350, 450 }
            };

            foreach (var row in mazePattern)
            {
                for (int i = 0; i < row.Length; i += 2)
                {
                    CreateObstacle(world, row[i], row[i + 1], "tree");
                }
            }
        }

        private void CreateEnemy(World world, float x, float y, string aiType, int health, int attack, float speed)
        {
            var enemy = world.CreateEntity();
            world.AddComponent(enemy, new Transform(x, y));
            world.AddComponent(enemy, new Velocity());
            world.AddComponent(enemy, new CombatStats(health, attack, 3));
            world.AddComponent(enemy, new EnemyAIComponent(aiType, 140f, 25f, speed, speed * 1.5f));
            world.AddComponent(enemy, new ColliderComponent(15, 15, true, "enemy"));
        }

        private void CreateObstacle(World world, float x, float y, string type)
        {
            var obstacle = world.CreateEntity();
            world.AddComponent(obstacle, new Transform(x, y));
            world.AddComponent(obstacle, new ObstacleComponent(type, false, 100));
            world.AddComponent(obstacle, new ColliderComponent(20, 20, true, type));
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
