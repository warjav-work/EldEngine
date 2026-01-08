using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.Core.Infrastructure.Logging;
using EldEngine.GameTest.Components;
using EldEngine.GameTest.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.GameTest.Scenes
{
    public class LevelOne : IScene
    {
        private List<Entity> _enemies = new();

        public string Name => "Level_01_Forest";

        public void Initialize(World world)
        {
            // 1. Crear jugador
            CreatePlayer(world);

            // 2. Crear NPCs
            CreateNpcs(world);

            // 3. Crear enemigos
            CreateEnemies(world);

            // 4. Crear escenario
            CreateEnvironment(world);

            // 5. Configurar listeners de eventos
            SetupEventListeners(world);

            Logger.Log($"Escena '{Name}' inicializada");
        }

        private void CreatePlayer(World world)
        {
            var player = world.CreateEntity();
            world.AddComponent(player, new Transform(50, 50));
            world.AddComponent(player, new Velocity());
            world.AddComponent(player, new PlayerController());
            world.AddComponent(player, new CombatStats(100, 20, 5));
            world.AddComponent(player, new InventoryComponent());
        }

        private void CreateNpcs(World world)
        {
            var npc1 = world.CreateEntity();
            world.AddComponent(npc1, new Transform(100, 100));
            world.AddComponent(npc1, new NpcBehavior("Elara", "dialogue.elara_intro"));

            var npc2 = world.CreateEntity();
            world.AddComponent(npc2, new Transform(150, 80));
            world.AddComponent(npc2, new NpcBehavior("Theron", "dialogue.theron_quest"));
        }

        private void CreateEnemies(World world)
        {
            for (int i = 0; i < 5; i++)
            {
                var enemy = world.CreateEntity();
                world.AddComponent(enemy, new Transform(200 + i * 30, 150));
                world.AddComponent(enemy, new Velocity());
                world.AddComponent(enemy, new CombatStats(30, 10, 2));
                _enemies.Add(enemy);
            }
        }

        private void CreateEnvironment(World world)
        {
            // Crear objetos estáticos del escenario
            for (int i = 0; i < 10; i++)
            {
                var tree = world.CreateEntity();
                world.AddComponent(tree, new Transform(300 + i * 50, 200));
                // Añadir componentes visuales
            }
        }

        private void SetupEventListeners(World world)
        {
            // Suscribirse a eventos de sistemas
            var combatSystem = world.GetSystem<CombatSystem>();
            if (combatSystem != null)
            {
                combatSystem.OnEntityDefeated += OnEnemyDefeated;
            }
        }

        private void OnEnemyDefeated(Entity entity)
        {
            Logger.Log($"Enemigo derrotado");
            _enemies.Remove(entity);

            if (_enemies.Count == 0)
                Logger.Log("¡Todos los enemigos derrotados!");
        }

        public void Cleanup(World world)
        {
            foreach (var enemy in _enemies)
                world.DestroyEntity(enemy);

            Logger.Log($"Escena '{Name}' limpiada");
        }
    }
}
