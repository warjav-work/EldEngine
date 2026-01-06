using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Scenes
{
    /// <summary>
    /// Nivel 1: Bosque Élfico - Introducción.
    /// </summary>
    public class Level01_Forest : IScene
    {
        public string Name => "Level_01_Forest";

        public void Initialize(World world)
        {
            System.Diagnostics.Debug.WriteLine("\n╔════════════════════════════════════╗");
            System.Diagnostics.Debug.WriteLine("║  NIVEL 1: BOSQUE ÉLFICO            ║");
            System.Diagnostics.Debug.WriteLine("║  Derrota 3 goblins para continuar  ║");
            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════╝\n");

            // Crear jugador
            var player = world.CreateEntity();
            world.AddComponent(player, new Transform(50, 400));
            world.AddComponent(player, new Velocity());
            world.AddComponent(player, new PlayerController(5f));
            world.AddComponent(player, new CombatStats(100, 15, 5));
            world.AddComponent(player, new Inventory());
            world.AddComponent(player, new ColliderComponent(16, 16, true, "player"));

            // Crear NPCs amigables (Aldea élfica)
            CreateNPC(world, 150, 150, "Arandor", "El Sabio");
            CreateNPC(world, 200, 200, "Elowen", "La Arquera");

            // Crear enemigos (Goblins) - Patrulla
            CreateEnemy(world, 300, 100, "patrol", 20, 8, 2);
            CreateEnemy(world, 400, 150, "patrol", 20, 8, 2);
            CreateEnemy(world, 350, 250, "patrol", 20, 8, 2);

            // Crear obstáculos (Árboles)
            CreateObstacle(world, 250, 300, "tree");
            CreateObstacle(world, 450, 350, "tree");
            CreateObstacle(world, 300, 450, "tree");

            // Crear paredes del mapa
            CreateWalls(world, 600, 500); // Tamaño del mapa

            System.Diagnostics.Debug.WriteLine("✓ Nivel 1 cargado (Elementos: 1 Jugador + 3 Enemigos + 2 NPCs + 5 Árboles)");
        }

        public void Cleanup(World world)
        {
            System.Diagnostics.Debug.WriteLine("🔄 Nivel 1 limpiado");
        }

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
            world.AddComponent(enemy, new CombatStats(health, attack, 2));
            world.AddComponent(enemy, new EnemyAIComponent(aiType, 120f, 20f, speed, speed * 1.5f));
            world.AddComponent(enemy, new ColliderComponent(14, 14, true, "enemy"));
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
            // Pared arriba
            var wallTop = world.CreateEntity();
            world.AddComponent(wallTop, new Transform(mapWidth / 2, -10));
            world.AddComponent(wallTop, new ObstacleComponent("wall", false, 1000));
            world.AddComponent(wallTop, new ColliderComponent(mapWidth, 20, true, "wall"));

            // Pared abajo
            var wallBottom = world.CreateEntity();
            world.AddComponent(wallBottom, new Transform(mapWidth / 2, mapHeight + 10));
            world.AddComponent(wallBottom, new ObstacleComponent("wall", false, 1000));
            world.AddComponent(wallBottom, new ColliderComponent(mapWidth, 20, true, "wall"));

            // Pared izquierda
            var wallLeft = world.CreateEntity();
            world.AddComponent(wallLeft, new Transform(-10, mapHeight / 2));
            world.AddComponent(wallLeft, new ObstacleComponent("wall", false, 1000));
            world.AddComponent(wallLeft, new ColliderComponent(20, mapHeight, true, "wall"));

            // Pared derecha
            var wallRight = world.CreateEntity();
            world.AddComponent(wallRight, new Transform(mapWidth + 10, mapHeight / 2));
            world.AddComponent(wallRight, new ObstacleComponent("wall", false, 1000));
            world.AddComponent(wallRight, new ColliderComponent(20, mapHeight, true, "wall"));
        }
    }
}
