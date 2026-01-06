using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Movimiento que respeta colisiones sólidas.
    /// </summary>
    public class ImprovedMovementSystem : ISystem
    {
        private CollisionSystem _collisionSystem;

        public string Name => nameof(ImprovedMovementSystem);
        public int Priority => 22;

        public ImprovedMovementSystem(CollisionSystem collisionSystem)
        {
            _collisionSystem = collisionSystem;
        }

        public void Execute(World world, float deltaTime)
        {
            var entities = world.GetEntitiesWith<Transform, Velocity>().ToList();

            foreach (var entity in entities)
            {
                var transform = world.GetComponent<Transform>(entity);
                var velocity = world.GetComponent<Velocity>(entity);

                // Calcular nueva posición
                float newX = transform.X + velocity.X * deltaTime;
                float newY = transform.Y + velocity.Y * deltaTime;

                // Validar colisión solo si tiene collider sólido
                if (world.HasComponent<ColliderComponent>(entity))
                {
                    // Intentar mover en X
                    if (_collisionSystem.CanMoveTo(world, entity, newX, transform.Y))
                    {
                        transform.X = newX;
                    }

                    // Intentar mover en Y
                    if (_collisionSystem.CanMoveTo(world, entity, transform.X, newY))
                    {
                        transform.Y = newY;
                    }
                }
                else
                {
                    // Sin colider, mover libremente
                    transform.X = newX;
                    transform.Y = newY;
                }

                world.RemoveComponent<Transform>(entity);
                world.AddComponent(entity, transform);
            }
        }
    }
}
