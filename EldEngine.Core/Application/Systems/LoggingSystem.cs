using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using System.Diagnostics;

namespace EldEngine.Core.Application.Systems
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 1: Sistema simple que hereda de SystemBase
    /// ═════════════════════════════════════════════════════════════════════════

    public class LoggingSystem : SystemBase
    {
        public override string Name => "LoggingSystem";
        public override int Priority => 200;  // Al final

        protected override void OnExecute(World world, float deltaTime)
        {
            // Loguear información cada N frames
            if (ExecutionCount % 60 == 0)  // Cada 1 segundo a 60 FPS
            {
                var entityCount = world.GetEntitiesWith<Transform>().Count();
                Debug.WriteLine($"[FRAME {ExecutionCount}] Entities: {entityCount}");
            }
        }
    }
}
