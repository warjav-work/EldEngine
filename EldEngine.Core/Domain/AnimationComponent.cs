using EldEngine.Core.Domain.Entities;

namespace EldEngine.Core.Domain
{
    public struct AnimationComponent : IComponent
    {
        /// <summary>
        /// Nombre del conjunto de animación
        /// Ejemplo: "idle", "walk", "jump", "attack"
        /// </summary>
        public string AnimationName { get; set; }

        /// <summary>
        /// Frame actual en la animación (0-indexed)
        /// </summary>
        public int CurrentFrame { get; set; }

        /// <summary>
        /// Tiempo acumulado desde el último frame
        /// </summary>
        public float FrameTimer { get; set; }

        /// <summary>
        /// Duración de cada frame en segundos
        /// </summary>
        public float FrameDuration { get; set; }

        /// <summary>
        /// Total de frames en la animación actual
        /// </summary>
        public int TotalFrames { get; set; }

        /// <summary>
        /// ¿Está la animación en reproducción?
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// ¿Se repite al terminar?
        /// </summary>
        public bool IsLooping { get; set; }

        /// <summary>
        /// Velocidad de reproducción (1.0 = normal)
        /// </summary>
        public float PlaybackSpeed { get; set; }

        public AnimationComponent(string animationName, int totalFrames,
            float frameDuration = 0.1f, bool isLooping = true)
        {
            AnimationName = animationName;
            CurrentFrame = 0;
            FrameTimer = 0f;
            FrameDuration = frameDuration;
            TotalFrames = totalFrames;
            IsPlaying = true;
            IsLooping = isLooping;
            PlaybackSpeed = 1.0f;
        }

        public void Play()
        {
            IsPlaying = true;
            CurrentFrame = 0;
            FrameTimer = 0f;
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            IsPlaying = true;
        }

        static int IComponent.GetComponentTypeId() =>
            typeof(AnimationComponent).GetHashCode();
    }
}
