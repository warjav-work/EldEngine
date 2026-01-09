using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using System.Diagnostics;

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
        private const float COLLISION_MARGIN = 0.5f;
        private const float OVERLAP_SEPARATION = 0.5f; // Separar overlaps
        private const float EPSILON = 0.01f;

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

        /// <summary>
        /// Aplica movimiento con colisión de deslizamiento (sliding collision).
        /// Maneja overlaps y permite deslizar por paredes.
        /// </summary>
        private void ApplyMovementWithSliding(World world, Entity entity, ref Transform transform,
            ref Velocity velocity, float deltaTime)
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
            // PASO 1: Intentar movimiento completo
            // ════════════════════════════════════════════════════════

            float newX = transform.X + velocity.X * deltaTime;
            float newY = transform.Y + velocity.Y * deltaTime;

            // Intentar mover a la nueva posición
            var (movedX, movedY, canMoveX, canMoveY) = TryMoveWithSliding(
                world, entity, transform.X, transform.Y, newX, newY, collider);

            transform.X = movedX;
            transform.Y = movedY;

            // ════════════════════════════════════════════════════════
            // PASO 2: Actualizar velocidad según colisiones
            // ════════════════════════════════════════════════════════

            if (!canMoveX)
            {
                velocity.X = 0; // Bloquear movimiento horizontal
            }

            if (!canMoveY)
            {
                velocity.Y = 0; // Bloquear movimiento vertical
            }

            // ════════════════════════════════════════════════════════
            // PASO 3: Resolver overlaps (si está dentro de algo)
            // ════════════════════════════════════════════════════════

            ResolveOverlaps(world, entity, ref transform, collider);

            Debug.WriteLine($"[MOVE] Pos({transform.X:F1}, {transform.Y:F1}) " +
                $"Vel({velocity.X:F2}, {velocity.Y:F2}) | " +
                $"Slide X={canMoveX}, Y={canMoveY}");
        }

        /// <summary>
        /// Intenta mover con sliding: primero ambos ejes, luego cada uno por separado.
        /// Retorna la posición final y si pudo moverse en cada eje.
        /// </summary>
        private (float x, float y, bool canMoveX, bool canMoveY) TryMoveWithSliding(
            World world, Entity entity, float currentX, float currentY, float newX, float newY,
            ColliderComponent collider)
        {
            // Caso 1: Intenta mover en ambos ejes
            if (CanMoveTo(world, entity, newX, newY, collider))
            {
                return (newX, newY, true, true);
            }

            // Caso 2: Intenta mover solo en X
            bool canMoveX = CanMoveTo(world, entity, newX, currentY, collider);
            float finalX = canMoveX ? newX : currentX;

            // Caso 3: Intenta mover solo en Y
            bool canMoveY = CanMoveTo(world, entity, currentX, newY, collider);
            float finalY = canMoveY ? newY : currentY;

            // Si puede mover en al menos un eje, hazlo (sliding)
            if (canMoveX || canMoveY)
            {
                return (finalX, finalY, canMoveX, canMoveY);
            }

            // Caso 4: No puede mover en ningún eje - intenta moverse un poco
            // para evitar quedarse pegado
            if (CanMoveTo(world, entity, currentX + EPSILON * 2, currentY, collider))
            {
                return (currentX + EPSILON * 2, currentY, false, false);
            }

            if (CanMoveTo(world, entity, currentX, currentY + EPSILON * 2, collider))
            {
                return (currentX, currentY + EPSILON * 2, false, false);
            }

            // Completamente bloqueado
            return (currentX, currentY, false, false);
        }

        /// <summary>
        /// Verifica si se puede mover a una posición sin colisionar con sólidos.
        /// </summary>
        private bool CanMoveTo(World world, Entity entity, float newX, float newY,
            ColliderComponent entityCollider)
        {
            var testTransform = new Transform { X = newX, Y = newY };

            var obstacles = world.GetEntitiesWith<Transform, ColliderComponent>().ToList();

            foreach (var obstacle in obstacles)
            {
                // Saltar la entidad misma
                if (obstacle.Id == entity.Id)
                    continue;

                if (!obstacle.IsValid)
                    continue;

                var obsCollider = world.GetComponent<ColliderComponent>(obstacle);

                // Solo colisionar con sólidos
                if (!obsCollider.IsSolid)
                    continue;

                var obsTransform = world.GetComponent<Transform>(obstacle);

                // Verificar colisión AABB con margen
                if (IsAABBColliding(testTransform, entityCollider, obsTransform, obsCollider))
                {
                    return false; // Movimiento bloqueado
                }
            }

            return true; // Movimiento permitido
        }

        /// <summary>
        /// Resuelve overlaps cuando una entidad está dentro de un obstáculo.
        /// Empuja la entidad hacia afuera.
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

                // Si está colisionando, empujar hacia afuera
                if (IsAABBColliding(transform, entityCollider, obsTransform, obsCollider))
                {
                    // Calcular el vector de separación (empuje mínimo)
                    var separation = CalculateSeparationVector(transform, entityCollider,
                        obsTransform, obsCollider);

                    transform.X += separation.x;
                    transform.Y += separation.y;

                    Debug.WriteLine($"[OVERLAP] Resolviendo overlap con {obsCollider.Tag} | " +
                        $"Empuje({separation.x:F2}, {separation.y:F2})");
                }
            }
        }

        /// <summary>
        /// Calcula el vector de separación mínimo para resolver un overlap.
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

            // Calcular penetración en cada eje
            float overlapLeft = right1 - left2;
            float overlapRight = right2 - left1;
            float overlapTop = bottom1 - top2;
            float overlapBottom = bottom2 - top1;

            // Encontrar la separación mínima
            float minOverlap = Math.Min(
                Math.Min(overlapLeft, overlapRight),
                Math.Min(overlapTop, overlapBottom)
            );

            // Empujar en la dirección del menor overlap
            if (minOverlap == overlapLeft)
                return (-overlapLeft - OVERLAP_SEPARATION, 0); // Empujar izquierda
            else if (minOverlap == overlapRight)
                return (overlapRight + OVERLAP_SEPARATION, 0); // Empujar derecha
            else if (minOverlap == overlapTop)
                return (0, -overlapTop - OVERLAP_SEPARATION); // Empujar arriba
            else
                return (0, overlapBottom + OVERLAP_SEPARATION); // Empujar abajo
        }

        /// <summary>
        /// Verifica colisión AABB entre dos rectángulos con margen de seguridad.
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

            // No colisiona si está completamente fuera en cualquier eje
            bool noCollision = right1 < left2 - COLLISION_MARGIN ||
                               left1 > right2 + COLLISION_MARGIN ||
                               bottom1 < top2 - COLLISION_MARGIN ||
                               top1 > bottom2 + COLLISION_MARGIN;

            return !noCollision;
        }
    }
}
