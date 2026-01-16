using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Worlds;
using System.Diagnostics;

namespace EldEngine.Core.Application.Systems
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 2: Sistema con lógica más compleja
    /// ═════════════════════════════════════════════════════════════════════════

    public class PerformanceMonitorSystem : SystemBase
    {
        public override string Name => "PerformanceMonitor";
        public override int Priority => 199;  // Antes de LoggingSystem

        private List<SystemBase> _systemsToMonitor = new();
        private float _logInterval = 5f;  // Segundos
        private float _timeSinceLastLog = 0f;

        public void AddSystemToMonitor(SystemBase system)
        {
            _systemsToMonitor.Add(system);
        }

        protected override void OnExecute(World world, float deltaTime)
        {
            _timeSinceLastLog += deltaTime;

            // Log cada 5 segundos
            if (_timeSinceLastLog >= _logInterval)
            {
                LogPerformance();
                _timeSinceLastLog = 0f;
            }
        }

        private void LogPerformance()
        {
            Debug.WriteLine("\n╔════════════════════════════════════════╗");
            Debug.WriteLine("║  PERFORMANCE REPORT                    ║");
            Debug.WriteLine("╠════════════════════════════════════════╣");

            foreach (var system in _systemsToMonitor)
            {
                if (system is SystemBase sysBase)
                {
                    Debug.WriteLine($"║ {sysBase.GetPerformanceStats().PadRight(37)} ║");
                }
            }

            Debug.WriteLine("╚════════════════════════════════════════╝\n");
        }
    }
}
