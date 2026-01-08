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
    /// Sistema de movimiento simple con dampening de velocidad
    /// Más simple que sliding, pero menos fluido
    /// </summary>
    public class DampeningMovementSystem : ISystem
    {
        private CollisionSystem _collisionSystem;

        public string Name => nameof(DampeningMovementSystem);
        public int Priority => 22;

        public DampeningMovementSystem(CollisionSystem collisionSystem)
        {
            _collisionSystem = collisionSystem ?? throw new ArgumentNullException(nameof(collisionSystem));
        }

        public void Execute(World world, float deltaTime)
        {
            var entities = world.GetEntitiesWith<Transform, Velocity>().ToList();

            foreach (var entity in entities)
            {
                var transform = world.GetComponent<Transform>(entity);
                var velocity = world.GetComponent<Velocity>(entity);

                float newX = transform.X + velocity.X * deltaTime;
                float newY = transform.Y + velocity.Y * deltaTime;

                if (world.HasComponent<ColliderComponent>(entity))
                {
                    var collider = world.GetComponent<ColliderComponent>(entity);

                    if (collider.IsSolid)
                    {
                        // ← SIMPLE: Si no puede moverse, poner velocidad a 0
                        if (!_collisionSystem.CanMoveTo(world, entity, newX, transform.Y))
                        {
                            velocity.X = 0; // DETENER en X
                        }
                        else
                        {
                            transform.X = newX;
                        }

                        if (!_collisionSystem.CanMoveTo(world, entity, transform.X, newY))
                        {
                            velocity.Y = 0; // DETENER en Y
                        }
                        else
                        {
                            transform.Y = newY;
                        }
                    }
                    else
                    {
                        transform.X = newX;
                        transform.Y = newY;
                    }
                }
                else
                {
                    transform.X = newX;
                    transform.Y = newY;
                }

                world.RemoveComponent<Transform>(entity);
                world.AddComponent(entity, transform);

                world.RemoveComponent<Velocity>(entity);
                world.AddComponent(entity, velocity);
            }
        }
    }
}
