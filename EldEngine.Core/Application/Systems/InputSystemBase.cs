using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Worlds;

namespace EldEngine.Core.Application.Systems
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 3: Sistema con input
    /// ═════════════════════════════════════════════════════════════════════════

    public abstract class InputSystemBase : SystemBase
    {
        public override int Priority => 10;  // Alto - procesar input primero

        protected abstract void OnInputUpdate(World world, float deltaTime);

        protected sealed override void OnExecute(World world, float deltaTime)
        {
            OnInputUpdate(world, deltaTime);
        }
    }
}
