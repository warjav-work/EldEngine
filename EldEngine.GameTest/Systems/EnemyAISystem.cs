using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Controla la IA de los enemigos.
    /// </summary>
    public class EnemyAISystem : ISystem
    {
        private CollisionSystem _collisionSystem;

        public string Name => nameof(EnemyAISystem);
        public int Priority => 45;

        public EnemyAISystem(CollisionSystem collisionSystem)
        {
            _collisionSystem = collisionSystem ?? throw new ArgumentNullException(nameof(collisionSystem));
        }

        public void Execute(World world, float deltaTime)
        {
            // Obtener jugador
            var players = world.GetEntitiesWith<PlayerController, Transform>().ToList();
            if (players.Count == 0) return;

            var player = players[0];
            var playerTransform = world.GetComponent<Transform>(player);

            // Procesar cada enemigo
            var enemies = world.GetEntitiesWith<EnemyAIComponent, Transform, Velocity>().ToList();

            foreach (var enemy in enemies)
            {
                if (!enemy.IsValid) continue;

                var aiComponent = world.GetComponent<EnemyAIComponent>(enemy);
                var enemyTransform = world.GetComponent<Transform>(enemy);
                var velocity = world.GetComponent<Velocity>(enemy);

                // Calcular distancia al jugador
                var dx = playerTransform.X - enemyTransform.X;
                var dy = playerTransform.Y - enemyTransform.Y;
                var distance = Math.Sqrt(dx * dx + dy * dy);

                // Actualizar visión del jugador
                aiComponent.CanSeePlayer = distance < aiComponent.DetectionRange;

                // Ejecutar comportamiento según tipo
                switch (aiComponent.AIType)
                {
                    case "patrol":
                        PatrolBehavior(world, enemy, ref aiComponent, ref enemyTransform, ref velocity, deltaTime, distance, playerTransform);
                        break;

                    case "follow":
                        FollowBehavior(world, enemy, ref aiComponent, ref enemyTransform, ref velocity, deltaTime, distance, playerTransform);
                        break;

                    case "aggressive":
                        AggressiveBehavior(world, enemy, ref aiComponent, ref enemyTransform, ref velocity, deltaTime, distance, playerTransform);
                        break;
                }

                // Actualizar componentes
                world.RemoveComponent<EnemyAIComponent>(enemy);
                world.AddComponent(enemy, aiComponent);

                world.RemoveComponent<Velocity>(enemy);
                world.AddComponent(enemy, velocity);

                world.RemoveComponent<Transform>(enemy);
                world.AddComponent(enemy, enemyTransform);
            }
        }

        private void PatrolBehavior(World world, Entity enemy, ref EnemyAIComponent ai, ref Transform transform, ref Velocity velocity, float deltaTime, double distance, Transform playerTransform)
        {
            if (ai.CanSeePlayer)
            {
                // Ve al jugador: cambiar a persecución
                ai.AIType = "follow";
                System.Diagnostics.Debug.WriteLine($"[AI] Enemigo {enemy.Id} detectó al jugador");
            }
            else
            {
                // Patrullar: movimiento aleatorio
                ai.StateChangeTime += deltaTime;
                if (ai.StateChangeTime > 3f)
                {
                    ai.StateChangeTime = 0;
                    velocity.X = (float)(new Random().NextDouble() - 0.5) * ai.PatrolSpeed * 2;
                    velocity.Y = (float)(new Random().NextDouble() - 0.5) * ai.PatrolSpeed * 2;
                }

                // Aplicar movimiento si es posible
                float newX = transform.X + velocity.X * deltaTime;
                float newY = transform.Y + velocity.Y * deltaTime;

                if (_collisionSystem.CanMoveTo(world, enemy, newX, newY))
                {
                    transform.X = newX;
                    transform.Y = newY;
                }
            }
        }

        private void FollowBehavior(World world, Entity enemy, ref EnemyAIComponent ai, ref Transform transform, ref Velocity velocity, float deltaTime, double distance, Transform playerTransform)
        {
            if (!ai.CanSeePlayer)
            {
                // Perdió al jugador: volver a patrulla
                ai.AIType = "patrol";
                velocity.X = 0;
                velocity.Y = 0;
                return;
            }

            // Perseguir al jugador
            var dx = playerTransform.X - transform.X;
            var dy = playerTransform.Y - transform.Y;
            var length = Math.Sqrt(dx * dx + dy * dy);

            if (length > 0.1)
            {
                velocity.X = (float)(dx / length) * ai.ChaseSpeed;
                velocity.Y = (float)(dy / length) * ai.ChaseSpeed;
            }

            // Aplicar movimiento
            float newX = transform.X + velocity.X * deltaTime;
            float newY = transform.Y + velocity.Y * deltaTime;

            if (_collisionSystem.CanMoveTo(world, enemy, newX, newY))
            {
                transform.X = newX;
                transform.Y = newY;
            }
        }

        private void AggressiveBehavior(World world, Entity enemy, ref EnemyAIComponent ai, ref Transform transform, ref Velocity velocity, float deltaTime, double distance, Transform playerTransform)
        {
            if (distance < ai.AttackRange)
            {
                // Atacar
                velocity.X = 0;
                velocity.Y = 0;
                System.Diagnostics.Debug.WriteLine($"[AI] Enemigo {enemy.Id} ataca");
            }
            else if (ai.CanSeePlayer)
            {
                // Perseguir
                var dx = playerTransform.X - transform.X;
                var dy = playerTransform.Y - transform.Y;
                var length = Math.Sqrt(dx * dx + dy * dy);

                if (length > 0.1)
                {
                    velocity.X = (float)(dx / length) * ai.ChaseSpeed;
                    velocity.Y = (float)(dy / length) * ai.ChaseSpeed;
                }

                // Aplicar movimiento
                float newX = transform.X + velocity.X * deltaTime;
                float newY = transform.Y + velocity.Y * deltaTime;

                if (_collisionSystem.CanMoveTo(world, enemy, newX, newY))
                {
                    transform.X = newX;
                    transform.Y = newY;
                }
            }
            else
            {
                // Patrullar
                velocity.X = 0;
                velocity.Y = 0;
            }
        }
    }
}
