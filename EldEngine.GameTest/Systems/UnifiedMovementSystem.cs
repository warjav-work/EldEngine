using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using System.Diagnostics;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Sistema de movimiento UNIFICADO que maneja:
    /// - Movimiento libre (sin colisiones)
    /// - Movimiento simple (bloquea completamente)
    /// - Movimiento deslizante (sliding - sube por paredes)
    /// 
    /// ANTES: 2 sistemas separados (ImprovedMovementSystem + SlidingMovementSystem)
    /// DESPUÉS: 1 sistema flexible con modo configurable
    /// 
    /// Esto reduce:
    /// - Código duplicado
    /// - Confusión sobre cuál usar
    /// - Overhead de tener 2 sistemas
    /// - Mantenimiento
    /// </summary>
    public class UnifiedMovementSystem : ISystem
    {
        public enum CollisionMode
        {
            None,       // Sin colisiones
            Simple,     // Bloquea completamente
            Sliding     // Permite deslizarse por paredes
        }

        private readonly CollisionSystem _collisionSystem;
        private List<Entity> _cachedEntities = new();
        private CollisionMode _mode = CollisionMode.Sliding;

        private const float COLLISION_MARGIN = 0.5f;
        private const float OVERLAP_SEPARATION = 0.5f;
        private const float EPSILON = 0.01f;

        public string Name => nameof(UnifiedMovementSystem);
        public int Priority => 22;
        public CollisionMode Mode
        {
            get => _mode;
            set => _mode = value;
        }

        public UnifiedMovementSystem(CollisionSystem collisionSystem = null, CollisionMode mode = CollisionMode.Sliding)
        {
            _collisionSystem = collisionSystem;
            _mode = mode;

            Debug.WriteLine($"[MovementSystem] ✓ Inicializado con modo: {mode}");
        }

        public void Execute(World world, float deltaTime)
        {
            // Usar lista cacheada para evitar ToList() cada frame
            _cachedEntities.Clear();
            foreach (var entity in world.GetEntitiesWith<Transform, Velocity>())
            {
                _cachedEntities.Add(entity);
            }

            foreach (var entity in _cachedEntities)
            {
                var transform = world.GetComponent<Transform>(entity);
                var velocity = world.GetComponent<Velocity>(entity);

                // Aplicar movimiento según modo
                ApplyMovement(world, entity, ref transform, ref velocity, deltaTime);

                // Actualizar componentes
                world.RemoveComponent<Transform>(entity);
                world.AddComponent(entity, transform);

                world.RemoveComponent<Velocity>(entity);
                world.AddComponent(entity, velocity);
            }
        }

        /// <summary>
        /// Aplica movimiento según el modo configurado.
        /// </summary>
        private void ApplyMovement(World world, Entity entity, ref Transform transform,
            ref Velocity velocity, float deltaTime)
        {
            switch (_mode)
            {
                case CollisionMode.None:
                    ApplyMovementNoCollision(ref transform, ref velocity, deltaTime);
                    break;

                case CollisionMode.Simple:
                    ApplyMovementSimple(world, entity, ref transform, ref velocity, deltaTime);
                    break;

                case CollisionMode.Sliding:
                    ApplyMovementSliding(world, entity, ref transform, ref velocity, deltaTime);
                    break;
            }
        }

        /// <summary>
        /// Movimiento libre sin colisiones.
        /// </summary>
        private void ApplyMovementNoCollision(ref Transform transform, ref Velocity velocity,
            float deltaTime)
        {
            transform.X += velocity.X * deltaTime;
            transform.Y += velocity.Y * deltaTime;
        }

        /// <summary>
        /// Movimiento simple - Bloquea completamente si hay obstáculo.
        /// </summary>
        private void ApplyMovementSimple(World world, Entity entity, ref Transform transform,
            ref Velocity velocity, float deltaTime)
        {
            if (_collisionSystem == null)
            {
                ApplyMovementNoCollision(ref transform, ref velocity, deltaTime);
                return;
            }

            if (!world.HasComponent<ColliderComponent>(entity))
            {
                ApplyMovementNoCollision(ref transform, ref velocity, deltaTime);
                return;
            }

            float newX = transform.X + velocity.X * deltaTime;
            float newY = transform.Y + velocity.Y * deltaTime;
            var collider = world.GetComponent<ColliderComponent>(entity);

            // Intentar mover en X
            if (_collisionSystem.CanMoveTo(world, entity, newX, transform.Y))
                transform.X = newX;
            else
                velocity.X = 0;

            // Intentar mover en Y
            if (_collisionSystem.CanMoveTo(world, entity, transform.X, newY))
                transform.Y = newY;
            else
                velocity.Y = 0;
        }

        /// <summary>
        /// Movimiento deslizante - Permite deslizarse por paredes.
        /// Primero intenta ambos ejes, luego cada uno por separado.
        /// </summary>
        private void ApplyMovementSliding(World world, Entity entity, ref Transform transform,
            ref Velocity velocity, float deltaTime)
        {
            if (_collisionSystem == null)
            {
                ApplyMovementNoCollision(ref transform, ref velocity, deltaTime);
                return;
            }

            if (!world.HasComponent<ColliderComponent>(entity))
            {
                ApplyMovementNoCollision(ref transform, ref velocity, deltaTime);
                return;
            }

            float newX = transform.X + velocity.X * deltaTime;
            float newY = transform.Y + velocity.Y * deltaTime;
            var collider = world.GetComponent<ColliderComponent>(entity);

            // Intentar mover en ambos ejes
            if (_collisionSystem.CanMoveTo(world, entity, newX, newY))
            {
                transform.X = newX;
                transform.Y = newY;
                return;
            }

            // Intentar solo X
            bool canMoveX = _collisionSystem.CanMoveTo(world, entity, newX, transform.Y);
            if (canMoveX)
                transform.X = newX;
            else
                velocity.X = 0;

            // Intentar solo Y
            bool canMoveY = _collisionSystem.CanMoveTo(world, entity, transform.X, newY);
            if (canMoveY)
                transform.Y = newY;
            else
                velocity.Y = 0;

            // Resolver overlaps si está dentro de algo
            if (!canMoveX && !canMoveY)
            {
                ResolveOverlaps(world, entity, ref transform, collider);
            }
        }

        /// <summary>
        /// Resuelve overlaps - Empuja entidad hacia afuera si está dentro de un obstáculo.
        /// </summary>
        private void ResolveOverlaps(World world, Entity entity, ref Transform transform,
            ColliderComponent entityCollider)
        {
            var obstacles = world.GetEntitiesWith<Transform, ColliderComponent>().ToList();

            foreach (var obstacle in obstacles)
            {
                if (obstacle.Id == entity.Id) continue;
                if (!obstacle.IsValid) continue;

                var obsCollider = world.GetComponent<ColliderComponent>(obstacle);
                if (!obsCollider.IsSolid) continue;

                var obsTransform = world.GetComponent<Transform>(obstacle);

                if (IsAABBColliding(transform, entityCollider, obsTransform, obsCollider))
                {
                    var separation = CalculateSeparationVector(transform, entityCollider,
                        obsTransform, obsCollider);

                    transform.X += separation.x;
                    transform.Y += separation.y;
                }
            }
        }

        /// <summary>
        /// Verifica colisión AABB.
        /// </summary>
        private bool IsAABBColliding(Transform t1, ColliderComponent c1, Transform t2, ColliderComponent c2)
        {
            float left1 = t1.X - c1.Width / 2f;
            float right1 = t1.X + c1.Width / 2f;
            float top1 = t1.Y - c1.Height / 2f;
            float bottom1 = t1.Y + c1.Height / 2f;

            float left2 = t2.X - c2.Width / 2f;
            float right2 = t2.X + c2.Width / 2f;
            float top2 = t2.Y - c2.Height / 2f;
            float bottom2 = t2.Y + c2.Height / 2f;

            bool noCollision = right1 < left2 - COLLISION_MARGIN ||
                               left1 > right2 + COLLISION_MARGIN ||
                               bottom1 < top2 - COLLISION_MARGIN ||
                               top1 > bottom2 + COLLISION_MARGIN;

            return !noCollision;
        }

        /// <summary>
        /// Calcula vector de separación mínimo.
        /// </summary>
        private (float x, float y) CalculateSeparationVector(Transform t1, ColliderComponent c1,
            Transform t2, ColliderComponent c2)
        {
            float left1 = t1.X - c1.Width / 2f;
            float right1 = t1.X + c1.Width / 2f;
            float top1 = t1.Y - c1.Height / 2f;
            float bottom1 = t1.Y + c1.Height / 2f;

            float left2 = t2.X - c2.Width / 2f;
            float right2 = t2.X + c2.Width / 2f;
            float top2 = t2.Y - c2.Height / 2f;
            float bottom2 = t2.Y + c2.Height / 2f;

            float overlapLeft = right1 - left2;
            float overlapRight = right2 - left1;
            float overlapTop = bottom1 - top2;
            float overlapBottom = bottom2 - top1;

            float minOverlap = Math.Min(
                Math.Min(overlapLeft, overlapRight),
                Math.Min(overlapTop, overlapBottom)
            );

            if (minOverlap == overlapLeft)
                return (-overlapLeft - OVERLAP_SEPARATION, 0);
            else if (minOverlap == overlapRight)
                return (overlapRight + OVERLAP_SEPARATION, 0);
            else if (minOverlap == overlapTop)
                return (0, -overlapTop - OVERLAP_SEPARATION);
            else
                return (0, overlapBottom + OVERLAP_SEPARATION);
        }
    }
}