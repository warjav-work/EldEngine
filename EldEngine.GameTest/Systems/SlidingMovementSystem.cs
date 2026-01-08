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
    /// Sistema de movimiento con sliding collision
    /// Permite deslizarse a lo largo de paredes en lugar de quedarse pegado
    /// </summary>
    public class SlidingMovementSystem : ISystem
    {
        private CollisionSystem _collisionSystem;
        private List<Entity> _cachedEntities = new();

        public string Name => nameof(SlidingMovementSystem);
        public int Priority => 22;

        public SlidingMovementSystem(CollisionSystem collisionSystem)
        {
            _collisionSystem = collisionSystem ?? throw new ArgumentNullException(nameof(collisionSystem));
        }

        public void Execute(World world, float deltaTime)
        {
            _cachedEntities.Clear();
            foreach (var entity in world.GetEntitiesWith<Transform, Velocity>())
            {
                _cachedEntities.Add(entity);
            }

            foreach (var entity in _cachedEntities)
            {
                var transform = world.GetComponent<Transform>(entity);
                var velocity = world.GetComponent<Velocity>(entity);

                // Aplicar movimiento
                ApplyMovementWithSliding(world, entity, ref transform, ref velocity, deltaTime);

                // Actualizar componentes
                world.RemoveComponent<Transform>(entity);
                world.AddComponent(entity, transform);

                world.RemoveComponent<Velocity>(entity);
                world.AddComponent(entity, velocity);
            }
        }

        // ← NUEVO: Aplicar movimiento con sliding collision
        private void ApplyMovementWithSliding(World world, Entity entity, ref Transform transform, ref Velocity velocity, float deltaTime)
        {
            // Si no tiene colider, mover libremente
            if (!world.HasComponent<ColliderComponent>(entity))
            {
                transform.X += velocity.X * deltaTime;
                transform.Y += velocity.Y * deltaTime;
                return;
            }

            var collider = world.GetComponent<ColliderComponent>(entity);

            // Si no es sólido, mover libremente
            if (!collider.IsSolid)
            {
                transform.X += velocity.X * deltaTime;
                transform.Y += velocity.Y * deltaTime;
                return;
            }

            // ════════════════════════════════════════════════════════
            // SLIDING COLLISION LOGIC
            // ════════════════════════════════════════════════════════

            float newX = transform.X + velocity.X * deltaTime;
            float newY = transform.Y + velocity.Y * deltaTime;

            // Intentar mover en ambos ejes
            bool canMoveX = _collisionSystem.CanMoveTo(world, entity, newX, transform.Y);
            bool canMoveY = _collisionSystem.CanMoveTo(world, entity, transform.X, newY);

            // CASO 1: Puede moverse en ambos ejes
            if (canMoveX && canMoveY)
            {
                transform.X = newX;
                transform.Y = newY;
                // La velocidad sigue igual (está "libre")
            }
            // CASO 2: Puede moverse solo en X
            else if (canMoveX && !canMoveY)
            {
                transform.X = newX;
                // ← CLAVE: Detener velocidad en Y (colisión detectada)
                velocity.Y = 0;

                if (_collisionSystem is CollisionSystem debugSys)
                    System.Diagnostics.Debug.WriteLine($"[SLIDE] Colisión en Y, deslizando en X");
            }
            // CASO 3: Puede moverse solo en Y
            else if (!canMoveX && canMoveY)
            {
                transform.Y = newY;
                // ← CLAVE: Detener velocidad en X (colisión detectada)
                velocity.X = 0;

                System.Diagnostics.Debug.WriteLine($"[SLIDE] Colisión en X, deslizando en Y");
            }
            // CASO 4: No puede moverse en ningún eje (esquina)
            else
            {
                // ← CLAVE: Detener completamente (no se mueve)
                velocity.X = 0;
                velocity.Y = 0;

                System.Diagnostics.Debug.WriteLine($"[COLLISION] Colisión en ambos ejes, DETENIDO");
            }
        }
    }
}
