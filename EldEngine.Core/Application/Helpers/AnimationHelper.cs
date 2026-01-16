using EldEngine.Core.Domain;
using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Application.Helpers
{
    public static class AnimationHelper
    {
        /// <summary>
        /// Cambiar la animación actual (recomienza desde frame 0)
        /// </summary>
        public static void ChangeAnimation(World world, Entity entity,
            string newAnimationName, int totalFrames,
            float frameDuration = 0.1f, bool isLooping = true)
        {
            if (!world.HasComponent<AnimationComponent>(entity))
            {
                world.AddComponent(entity, new AnimationComponent(
                    newAnimationName, totalFrames, frameDuration, isLooping));
                return;
            }

            var animation = world.GetComponent<AnimationComponent>(entity);
            animation.AnimationName = newAnimationName;
            animation.CurrentFrame = 0;
            animation.FrameTimer = 0f;
            animation.TotalFrames = totalFrames;
            animation.FrameDuration = frameDuration;
            animation.IsLooping = isLooping;
            animation.IsPlaying = true;

            world.RemoveComponent<AnimationComponent>(entity);
            world.AddComponent(entity, animation);
        }

        /// <summary>
        /// Pausar animación actual
        /// </summary>
        public static void PauseAnimation(World world, Entity entity)
        {
            if (!world.HasComponent<AnimationComponent>(entity))
                return;

            var animation = world.GetComponent<AnimationComponent>(entity);
            animation.IsPlaying = false;

            world.RemoveComponent<AnimationComponent>(entity);
            world.AddComponent(entity, animation);
        }

        /// <summary>
        /// Reanudar animación
        /// </summary>
        public static void ResumeAnimation(World world, Entity entity)
        {
            if (!world.HasComponent<AnimationComponent>(entity))
                return;

            var animation = world.GetComponent<AnimationComponent>(entity);
            animation.IsPlaying = true;

            world.RemoveComponent<AnimationComponent>(entity);
            world.AddComponent(entity, animation);
        }
    }
}
