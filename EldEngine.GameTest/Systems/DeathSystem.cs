using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using EldEngine.GameTest.GameStates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Sistema que detecta cuando entidades mueren (vida = 0 o menos).
    /// Cuando el jugador muere, dispara Game Over.
    /// Cuando enemigos mueren, suma puntos y los elimina.
    /// </summary>
    public class DeathSystem : ISystem
    {
        private GameManager _gameManager;
        private List<Entity> _deadEntities = new();

        public string Name => nameof(DeathSystem);
        public int Priority => 110; // DESPUÉS de combat (100)

        public event Action<Entity> OnEntityDeath;

        public DeathSystem(GameManager gameManager)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
        }

        public void Execute(World world, float deltaTime)
        {
            _deadEntities.Clear();

            // Obtener todas las entidades con CombatStats
            var entities = world.GetEntitiesWith<CombatStats>().ToList();

            foreach (var entity in entities)
            {
                if (!entity.IsValid) continue;

                var stats = world.GetComponent<CombatStats>(entity);

                // Verificar si está muerto
                if (stats.CurrentHealth <= 0)
                {
                    _deadEntities.Add(entity);
                }
            }

            // Procesar muertes
            foreach (var entity in _deadEntities)
            {
                HandleDeath(world, entity);
            }
        }

        /// <summary>
        /// Maneja la muerte de una entidad.
        /// </summary>
        private void HandleDeath(World world, Entity entity)
        {
            if (!entity.IsValid) return;

            // Verificar si es el jugador
            if (world.HasComponent<PlayerController>(entity))
            {
                HandlePlayerDeath(world, entity);
            }
            // Si es enemigo
            else if (world.HasComponent<EnemyAIComponent>(entity))
            {
                HandleEnemyDeath(world, entity);
            }

            // Disparar evento
            OnEntityDeath?.Invoke(entity);

            // Opcionalmente: Destruir la entidad inmediatamente
            // world.DestroyEntity(entity);
        }

        /// <summary>
        /// Maneja la muerte del jugador - Game Over.
        /// </summary>
        private void HandlePlayerDeath(World world, Entity player)
        {
            var stats = world.GetComponent<CombatStats>(player);

            Debug.WriteLine("\n╔════════════════════════════════════╗");
            Debug.WriteLine("║  ☠️  EL JUGADOR HA MUERTO  ☠️       ║");
            Debug.WriteLine($"║  Vida: {stats.CurrentHealth}/{stats.MaxHealth}                       ║");
            Debug.WriteLine("╚════════════════════════════════════╝\n");

            // Cambiar estado a Game Over
            _gameManager.GameOver("El jugador fue derrotado en combate");
        }

        /// <summary>
        /// Maneja la muerte de enemigos - Suma puntos y elimina.
        /// </summary>
        private void HandleEnemyDeath(World world, Entity enemy)
        {
            var stats = world.GetComponent<CombatStats>(enemy);
            var aiComponent = world.GetComponent<EnemyAIComponent>(enemy);

            // Puntos base por matar enemigo
            int pointsReward = 50;

            // Bonus por dificultad (vida * 5 puntos)
            pointsReward += stats.MaxHealth * 2;

            // Sumar puntos
            _gameManager.AddScore(pointsReward);

            Debug.WriteLine($"[DEATH] ☠️ Enemigo derrotado | +{pointsReward} puntos | " +
                $"ID: {enemy.Id}");

            // Mostrar mensaje flotante (si tienes sistema de mensajes)
            // TODO: _gameManager.OnGameMessage?.Invoke($"⭐ +{pointsReward} puntos!");

            // Destruir la entidad del enemigo
            world.DestroyEntity(enemy);
        }

        /// <summary>
        /// Obtiene el estado de salud de una entidad como porcentaje.
        /// Útil para mostrar barras de vida.
        /// </summary>
        public float GetHealthPercentage(World world, Entity entity)
        {
            if (!world.HasComponent<CombatStats>(entity))
                return 1f;

            var stats = world.GetComponent<CombatStats>(entity);
            return Math.Max(0, stats.CurrentHealth / (float)stats.MaxHealth);
        }

        /// <summary>
        /// Verifica si una entidad está viva.
        /// </summary>
        public bool IsAlive(World world, Entity entity)
        {
            if (!world.HasComponent<CombatStats>(entity))
                return true;

            var stats = world.GetComponent<CombatStats>(entity);
            return stats.CurrentHealth > 0;
        }
    }
}