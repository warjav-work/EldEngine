using System.Diagnostics;

namespace EldEngine.Core.Application.Services
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// ServiceBase - Clase Abstracta Base
    /// ═════════════════════════════════════════════════════════════════════════
    /// 
    /// VENTAJAS:
    /// ✅ Implementar métodos comunes de servicio una sola vez
    /// ✅ Ciclo de vida consistente (Initialize → Dispose)
    /// ✅ Manejo de errores centralizado
    /// ✅ Logging automático
    /// ✅ Estado del servicio
    ///
    public abstract class ServiceBase : IDisposable
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Nombre del servicio.
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// ¿Está el servicio inicializado?
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// ¿Está disposable?
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// Versión del servicio.
        /// </summary>
        public virtual string Version => "1.0";

        // ═══════════════════════════════════════════════════════════════════
        // EVENTOS
        // ═══════════════════════════════════════════════════════════════════

        public event Action<ServiceBase> OnInitialized;
        public event Action<ServiceBase> OnDisposed;
        public event Action<ServiceBase, Exception> OnError;

        // ═══════════════════════════════════════════════════════════════════
        // CICLO DE VIDA
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Inicializa el servicio.
        /// Llamar una sola vez al crear el servicio.
        /// </summary>
        public virtual void Initialize()
        {
            if (IsInitialized)
            {
                Debug.WriteLine($"[{Name}] Ya fue inicializado");
                return;
            }

            try
            {
                Debug.WriteLine($"[{Name}] Inicializando...");

                // Llamar a inicialización específica
                OnInitialize();

                IsInitialized = true;
                OnInitialized?.Invoke(this);

                Debug.WriteLine($"✓ [{Name}] Inicializado exitosamente");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
                Debug.WriteLine($"❌ [{Name}] Error durante inicialización: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método abstracto que cada servicio implementa.
        /// Se llama durante Initialize().
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// Limpia el servicio.
        /// Implementa IDisposable.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            try
            {
                Debug.WriteLine($"[{Name}] Limpiando...");

                // Llamar a limpieza específica
                OnDispose();

                IsDisposed = true;
                OnDisposed?.Invoke(this);

                Debug.WriteLine($"✓ [{Name}] Limpiado exitosamente");
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
                Debug.WriteLine($"❌ [{Name}] Error durante limpieza: {ex.Message}");
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Método abstracto que cada servicio implementa.
        /// Se llama durante Dispose().
        /// </summary>
        protected abstract void OnDispose();

        /// <summary>
        /// Verifica que el servicio esté inicializado.
        /// Lanza excepción si no.
        /// </summary>
        protected void AssertInitialized()
        {
            if (!IsInitialized)
                throw new InvalidOperationException($"{Name} no está inicializado");

            if (IsDisposed)
                throw new ObjectDisposedException(Name);
        }

        // ═══════════════════════════════════════════════════════════════════
        // INFORMACIÓN
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Obtiene información del servicio.
        /// </summary>
        public virtual string GetInfo()
        {
            return $"{Name} v{Version} | " +
                   $"Initialized: {IsInitialized} | " +
                   $"Disposed: {IsDisposed}";
        }

        /// <summary>
        /// Obtiene información del servicio.
        /// </summary>
        public override string ToString()
        {
            return GetInfo();
        }
    }
}
