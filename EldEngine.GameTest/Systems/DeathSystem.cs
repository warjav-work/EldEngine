using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.Core.Infrastructure.Caching;
using EldEngine.Core.Infrastructure.Events;
using EldEngine.GameTest.Components;
using EldEngine.GameTest.Events;
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
        private readonly EventBus _eventBus;
        private readonly SystemQueryCache _cache;
        private List<Entity> _deadEntities = new();

        public string Name => nameof(DeathSystem);
        public int Priority => 110; // DESPUÉS de combat (100)

        public event Action<Entity> OnEntityDeath;

        /// <summary>
        /// Crea un DeathSystem desacoplado.
        /// </summary>
        public DeathSystem(EventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _cache = new SystemQueryCache();
        }

        public void Execute(World world, float deltaTime)
        {
            _deadEntities.Clear();

            // Obtener todas las entidades con CombatStats (usar caché)
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

            // Procesar muertes (después de iterar sobre colecciones)
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
            // Marcar para destrucción
            // (Se destruye después de que todos los sistemas terminen)
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

            // ✅ NUEVO: Publicar evento en lugar de llamar GameManager
            _eventBus.Publish(new PlayerDiedEvent
            {
                Player = player,
                Reason = "El jugador fue derrotado en combate"
            });

        }

        /// <summary>
        /// Maneja la muerte de enemigos - Suma puntos y elimina.
        /// </summary>
        private void HandleEnemyDeath(World world, Entity enemy)
        {
            var stats = world.GetComponent<CombatStats>(enemy);            
            var transform = world.GetComponent<Transform>(enemy);

            // Calcular puntos
            int basePoints = 50;
            int healthBonus = stats.MaxHealth * 2;
            int totalPoints = basePoints + healthBonus;

            // Obtener tipo de enemigo
            string enemyType = "unknown";
            if (world.HasComponent<EnemyAIComponent>(enemy))
            {
                var ai = world.GetComponent<EnemyAIComponent>(enemy);
                enemyType = ai.AIType;
            }

            // Crear evento con información completa
            var enemyDefeatedEvent = new EnemyDefeatedEvent(
                enemy: enemy,
                pointsRewarded: totalPoints,
                enemyType: enemyType,
                maxHealth: stats.MaxHealth,
                attack: stats.Attack,
                defeatPosition: (transform.X, transform.Y),
                wasOneShot: false  // O calcular si fue one-shot
            );

            // ✅ Publicar evento
            _eventBus.Publish(enemyDefeatedEvent);

            Debug.WriteLine($"[DEATH] ☠️ Enemigo derrotado: {enemyDefeatedEvent}");

            // Destruir la entidad
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