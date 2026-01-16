using EldEngine.Core.Domain;
using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Application.Systems
{
    public class AnimationSystem : ISystem
    {
        public string Name => nameof(AnimationSystem);

        /// <summary>
        /// Ejecutar antes del renderizado
        /// </summary>
        public int Priority => 95;

        public void Execute(World world, float deltaTime)
        {
            try
            {
                // Obtener todas las entidades con animación
                var entities = world
                    .GetEntitiesWith<AnimationComponent, SpriteComponent>()
                    .ToList();

                foreach (var entity in entities)
                {
                    UpdateAnimation(world, entity, deltaTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AnimationSystem] Error: {ex.Message}");
            }
        }

        private void UpdateAnimation(World world, Entity entity,
            float deltaTime)
        {
            var animation = world.GetComponent<AnimationComponent>(entity);

            // Si no está reproduciendo, ignorar
            if (!animation.IsPlaying)
                return;

            // Actualizar timer
            animation.FrameTimer += deltaTime * animation.PlaybackSpeed;

            // Duración efectiva del frame
            float frameDuration = animation.FrameDuration;

            // Si la duración es mayor que el timer, no cambiar frame
            if (animation.FrameTimer < frameDuration)
            {
                world.RemoveComponent<AnimationComponent>(entity);
                world.AddComponent(entity, animation);
                return;
            }

            // Avanzar frame
            animation.FrameTimer -= frameDuration;
            animation.CurrentFrame++;

            // Verificar si se acabó la animación
            if (animation.CurrentFrame >= animation.TotalFrames)
            {
                if (animation.IsLooping)
                {
                    animation.CurrentFrame = 0;
                }
                else
                {
                    animation.CurrentFrame = animation.TotalFrames - 1;
                    animation.IsPlaying = false;
                }
            }

            // Actualizar componente
            world.RemoveComponent<AnimationComponent>(entity);
            world.AddComponent(entity, animation);
        }
    }
}
