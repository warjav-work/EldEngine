using EldEngine.Core.Domain.Worlds;
using System.Diagnostics;

namespace EldEngine.Core.Domain.Systems
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// SystemBase - Clase Abstracta Base
    /// ═════════════════════════════════════════════════════════════════════════
    /// 
    /// VENTAJAS:
    /// ✅ Implementar métodos comunes una sola vez
    /// ✅ Logging automático
    /// ✅ Performance tracking
    /// ✅ State management
    /// ✅ Error handling robusto
    ///
    public abstract class SystemBase : ISystem
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES BASE
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Nombre del sistema (debe ser sobrescrito).
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// Prioridad de ejecución (puede ser sobrescrita).
        /// </summary>
        public virtual int Priority => 50;

        /// <summary>
        /// ¿Está el sistema habilitado?
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Número de veces que se ha ejecutado.
        /// </summary>
        public long ExecutionCount { get; private set; } = 0;

        /// <summary>
        /// Tiempo total gastado en este sistema.
        /// </summary>
        public double TotalExecutionTime { get; private set; } = 0;

        /// <summary>
        /// Tiempo promedio por ejecución.
        /// </summary>
        public double AverageExecutionTime =>
            ExecutionCount > 0 ? TotalExecutionTime / ExecutionCount : 0;

        /// <summary>
        /// Último tiempo de ejecución.
        /// </summary>
        public double LastExecutionTime { get; private set; } = 0;

        // ═══════════════════════════════════════════════════════════════════
        // EVENTOS
        // ═══════════════════════════════════════════════════════════════════

        public event Action<SystemBase> OnExecutionStarted;
        public event Action<SystemBase, Exception> OnExecutionFailed;
        public event Action<SystemBase> OnExecutionCompleted;

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS BASE
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Implementación final de Execute().
        /// Maneja logging, timing, y error handling automáticamente.
        /// </summary>
        public void Execute(World world, float deltaTime)
        {
            // ¿Está deshabilitado? Saltar
            if (!IsEnabled)
                return;

            OnExecutionStarted?.Invoke(this);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // ✅ Llamar a la implementación específica del sistema
                OnExecute(world, deltaTime);

                stopwatch.Stop();
                LastExecutionTime = stopwatch.Elapsed.TotalMilliseconds;
                TotalExecutionTime += LastExecutionTime;
                ExecutionCount++;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                OnExecutionFailed?.Invoke(this, ex);

                Debug.WriteLine(
                    $"[ERROR] {Name} falló: {ex.Message}\n{ex.StackTrace}");

                throw;
            }

            OnExecutionCompleted?.Invoke(this);
        }

        /// <summary>
        /// Método abstracto que cada sistema debe implementar.
        /// Aquí va la lógica específica del sistema.
        /// </summary>
        protected abstract void OnExecute(World world, float deltaTime);

        /// <summary>
        /// Inicializa el sistema.
        /// Se llama cuando se registra en el SystemRegistry.
        /// </summary>
        public virtual void Initialize(World world)
        {
            Debug.WriteLine($"[INIT] {Name} inicializado (Priority: {Priority})");
        }

        /// <summary>
        /// Limpia recursos cuando se destruye el sistema.
        /// </summary>
        public virtual void Shutdown()
        {
            Debug.WriteLine($"[SHUTDOWN] {Name} limpiado");
        }

        /// <summary>
        /// Resetea estadísticas de performance.
        /// </summary>
        public void ResetStats()
        {
            ExecutionCount = 0;
            TotalExecutionTime = 0;
            LastExecutionTime = 0;
        }

        /// <summary>
        /// Obtiene información de performance del sistema.
        /// </summary>
        public string GetPerformanceStats()
        {
            return $"{Name} | " +
                   $"Executions: {ExecutionCount} | " +
                   $"Avg: {AverageExecutionTime:F3}ms | " +
                   $"Last: {LastExecutionTime:F3}ms | " +
                   $"Total: {TotalExecutionTime:F1}ms | " +
                   $"Enabled: {IsEnabled}";
        }

        /// <summary>
        /// Obtiene información general del sistema.
        /// </summary>
        public override string ToString()
        {
            return $"{Name} (Priority: {Priority}, Enabled: {IsEnabled})";
        }
    }
}
