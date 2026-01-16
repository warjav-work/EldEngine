using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Worlds;
using System.Diagnostics;

namespace EldEngine.Core.Application.Systems
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 5: Sistema con eventos
    /// ═════════════════════════════════════════════════════════════════════════

    public abstract class EventSystemBase : SystemBase
    {
        public override int Priority => 100;  // Gameplay logic

        public event Action<string> OnSystemEvent;

        protected void RaiseEvent(string eventMessage)
        {
            OnSystemEvent?.Invoke(eventMessage);
            Debug.WriteLine($"[{Name}] {eventMessage}");
        }

        protected abstract void OnEventUpdate(World world, float deltaTime);

        protected sealed override void OnExecute(World world, float deltaTime)
        {
            OnEventUpdate(world, deltaTime);
        }
    }
}
