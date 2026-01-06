using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Domain.Systems
{
    public class MovementSystem : ISystem
    {
        public string Name => nameof(MovementSystem);
        public int Priority => 100;

        public void Execute(World world, float deltaTime)
        {
            foreach (var entity in world.GetEntitiesWith<Transform, Velocity>().ToList())
            {
                var transform = world.GetComponent<Transform>(entity);
                var velocity = world.GetComponent<Velocity>(entity);

                // Aplicar velocidad
                transform.X += velocity.X * deltaTime;
                transform.Y += velocity.Y * deltaTime;

                // Actualizar componente
                world.RemoveComponent<Transform>(entity);
                world.AddComponent(entity, transform);
            }
        }
    }
}
