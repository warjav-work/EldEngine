using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Application.Systems
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 4: Sistema con física
    /// ═════════════════════════════════════════════════════════════════════════

    public abstract class PhysicsSystemBase : SystemBase
    {
        public override int Priority => 25;  // Después de input

        // ┌─────────────────────────────────────────────────────────┐
        // │ Propiedades comunes a todos los sistemas de física      │
        // └─────────────────────────────────────────────────────────┘

        public float Gravity { get; set; } = 9.81f;
        public float TimeScale { get; set; } = 1f;
        public bool IsPaused { get; set; } = false;

        protected abstract void OnPhysicsUpdate(World world, float deltaTime);

        protected sealed override void OnExecute(World world, float deltaTime)
        {
            if (IsPaused)
                return;

            // Aplicar time scale
            float adjustedDeltaTime = deltaTime * TimeScale;

            OnPhysicsUpdate(world, adjustedDeltaTime);
        }

        public override string ToString()
        {
            return $"{base.ToString()} | Gravity: {Gravity} | TimeScale: {TimeScale}";
        }
    }
}
