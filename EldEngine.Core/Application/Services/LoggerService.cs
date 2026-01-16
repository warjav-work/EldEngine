using System.Diagnostics;

namespace EldEngine.Core.Application.Services
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 1: Servicio simple
    /// ═════════════════════════════════════════════════════════════════════════

    public class LoggerService : ServiceBase
    {
        public override string Name => "Logger";

        private List<string> _logs = new();

        protected override void OnInitialize()
        {
            _logs.Clear();
            Debug.WriteLine("Logger inicializado");
        }

        protected override void OnDispose()
        {
            Debug.WriteLine($"Logger: {_logs.Count} mensajes registrados");
            _logs.Clear();
        }

        public void Log(string message)
        {
            AssertInitialized();
            _logs.Add(message);
            Debug.WriteLine($"[LOG] {message}");
        }

        public IReadOnlyList<string> GetLogs()
        {
            AssertInitialized();
            return _logs.AsReadOnly();
        }
    }
}
