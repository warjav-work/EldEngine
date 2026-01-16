using System.Diagnostics;

namespace EldEngine.Core.Application.Services
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 5: Manager de servicios
    /// ═════════════════════════════════════════════════════════════════════════

    public class ServiceManager : ServiceBase
    {
        public override string Name => "ServiceManager";

        private List<ServiceBase> _services = new();

        public void RegisterService(ServiceBase service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services.Add(service);
            Debug.WriteLine($"[{Name}] Servicio registrado: {service.Name}");
        }

        public T GetService<T>() where T : ServiceBase
        {
            return _services.OfType<T>().FirstOrDefault();
        }

        protected override void OnInitialize()
        {
            // Inicializar todos los servicios registrados
            foreach (var service in _services)
            {
                try
                {
                    service.Initialize();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error inicializando {service.Name}: {ex.Message}");
                    throw;
                }
            }
        }

        protected override void OnDispose()
        {
            // Limpiar servicios en orden inverso
            for (int i = _services.Count - 1; i >= 0; i--)
            {
                try
                {
                    _services[i].Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Error limpiando {_services[i].Name}: {ex.Message}");
                }
            }

            _services.Clear();
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()} | Services: {_services.Count}";
        }
    }
}
