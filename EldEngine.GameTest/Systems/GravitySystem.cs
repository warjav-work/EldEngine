using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Systems
{
    public class GravitySystem : ISystem
    {
        private const float GRAVITY = 9.81f;

        public string Name => nameof(GravitySystem);
        public int Priority => 25; // Después de input, antes de movimiento

        public void Execute(World world, float deltaTime)
        {
            var entities = world.GetEntitiesWith<GravityComponent, Velocity>();

            foreach (var entity in entities)
            {
                var gravity = world.GetComponent<GravityComponent>(entity);
                var velocity = world.GetComponent<Velocity>(entity);

                if (gravity.UseGravity)
                {
                    // Aplicar gravedad
                    velocity.Y += GRAVITY * gravity.GravityScale * deltaTime;
                }

                // Actualizar componente
                world.RemoveComponent<Velocity>(entity);
                world.AddComponent(entity, velocity);
            }
        }
    }
}
