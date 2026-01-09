using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Worlds;
using System.Diagnostics;

namespace EldEngine.Core.Infrastructure.Systems
{
    /// <summary>
    /// Registry centralizado para gestionar sistemas de forma desacoplada.
    /// 
    /// VENTAJAS:
    /// - GameWindow no crea sistemas directamente
    /// - Se pueden reemplazar sistemas sin tocar GameWindow
    /// - Fácil de testear
    /// - Escalable
    /// 
    /// ANTES:
    /// gameService.World.AddSystem(new PlayerInputSystem(input));
    /// gameService.World.AddSystem(new CollisionSystem());
    /// gameService.World.AddSystem(new EnemyAISystem(collision));
    /// 
    /// DESPUÉS:
    /// registry.Register<PlayerInputSystem>(() => new PlayerInputSystem(input));
    /// registry.Register<CollisionSystem>(() => new CollisionSystem());
    /// registry.Initialize();
    /// </summary>
    public class SystemRegistry
    {
        private readonly World _world;
        private readonly Dictionary<Type, Func<ISystem>> _factories = new();
        private readonly Dictionary<Type, ISystem> _instances = new();
        private readonly List<SystemRegistration> _registrations = new();
        private bool _initialized = false;

        public event Action<ISystem> OnSystemAdded;
        public event Action<ISystem> OnSystemRemoved;

        public SystemRegistry(World world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        /// <summary>
        /// Registra un factory para un sistema.
        /// El factory se ejecuta durante Initialize().
        /// </summary>
        public void Register<T>(Func<T> factory, int priority = 0) where T : ISystem
        {
            if (_initialized)
                throw new InvalidOperationException("No se pueden registrar sistemas después de Initialize()");

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factories[typeof(T)] = () => factory();
            _registrations.Add(new SystemRegistration(typeof(T), factory, priority));

            Debug.WriteLine($"[SystemRegistry] ✓ Registrado: {typeof(T).Name}");
        }

        /// <summary>
        /// Registra una instancia directa de un sistema.
        /// </summary>
        public void RegisterInstance<T>(T instance, int priority = 0) where T : ISystem
        {
            if (_initialized)
                throw new InvalidOperationException("No se pueden registrar sistemas después de Initialize()");

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            _instances[typeof(T)] = instance;
            _registrations.Add(new SystemRegistration(typeof(T), () => instance, priority));

            Debug.WriteLine($"[SystemRegistry] ✓ Instancia registrada: {typeof(T).Name}");
        }

        /// <summary>
        /// Obtiene una instancia de sistema registrado.
        /// </summary>
        public T GetSystem<T>() where T : ISystem
        {
            if (_instances.TryGetValue(typeof(T), out var system))
                return (T)system;

            throw new KeyNotFoundException($"Sistema no registrado: {typeof(T).Name}");
        }

        /// <summary>
        /// Intenta obtener un sistema sin lanzar excepción.
        /// </summary>
        public bool TryGetSystem<T>(out T? system) where T : ISystem
        {
            system = default;

            if (_instances.TryGetValue(typeof(T), out var s))
            {
                system = (T)s;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Obtiene todos los sistemas registrados.
        /// </summary>
        public IEnumerable<ISystem> GetAllSystems()
        {
            return _instances.Values;
        }

        /// <summary>
        /// Inicializa todos los sistemas registrados.
        /// Debe llamarse UNA SOLA VEZ antes de usar los sistemas.
        /// </summary>
        public void Initialize()
        {
            if (_initialized)
            {
                Debug.WriteLine("[SystemRegistry] ⚠️ Ya fue inicializado");
                return;
            }

            Debug.WriteLine("\n╔════════════════════════════════════╗");
            Debug.WriteLine("║  INICIALIZANDO REGISTRO DE SISTEMAS ║");
            Debug.WriteLine("╚════════════════════════════════════╝\n");

            try
            {
                // Ordenar por prioridad
                var sorted = _registrations.OrderBy(r => r.Priority).ToList();

                foreach (var registration in sorted)
                {
                    try
                    {
                        // Ejecutar factory
                        var system = (ISystem)registration.Factory.DynamicInvoke();

                        if (system == null)
                            throw new InvalidOperationException(
                                $"Factory retornó null para {registration.SystemType.Name}");

                        // Agregar al world
                        _world.AddSystem(system);
                        _instances[registration.SystemType] = system;

                        OnSystemAdded?.Invoke(system);

                        Debug.WriteLine(
                            $"✓ [{registration.Priority:D3}] {registration.SystemType.Name}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(
                            $"❌ Error inicializando {registration.SystemType.Name}: {ex.Message}");
                        throw;
                    }
                }

                _initialized = true;

                Debug.WriteLine(
                    $"\n✓ {_instances.Count} sistemas inicializados exitosamente\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n❌ Error durante inicialización: {ex.Message}\n");
                throw;
            }
        }

        /// <summary>
        /// Remueve un sistema en tiempo de ejecución.
        /// </summary>
        public bool RemoveSystem<T>() where T : ISystem
        {
            if (_instances.TryGetValue(typeof(T), out var system))
            {
                _world.RemoveSystem(system);
                _instances.Remove(typeof(T));
                _factories.Remove(typeof(T));

                OnSystemRemoved?.Invoke(system);

                Debug.WriteLine($"[SystemRegistry] ✓ Sistema removido: {typeof(T).Name}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reemplaza un sistema por otro.
        /// </summary>
        public void ReplaceSystem<T>(Func<T> factory) where T : ISystem
        {
            if (!_initialized)
                throw new InvalidOperationException("Debe inicializar primero");

            RemoveSystem<T>();

            var oldRegistration = _registrations.FirstOrDefault(r => r.SystemType == typeof(T));
            if (oldRegistration != null)
            {
                _registrations.Remove(oldRegistration);
            }

            var system = factory();
            _world.AddSystem(system);
            _instances[typeof(T)] = system;

            Debug.WriteLine($"[SystemRegistry] ✓ Sistema reemplazado: {typeof(T).Name}");
        }

        /// <summary>
        /// Obtiene estadísticas de sistemas (debugging).
        /// </summary>
        public string GetStats()
        {
            var systems = string.Join(", ",
                _instances.Keys.Select(t => t.Name));

            return $"SystemRegistry | Sistemas: {_instances.Count} | [{systems}] | " +
                   $"Inicializado: {_initialized}";
        }

        public override string ToString() => GetStats();

        private class SystemRegistration
        {
            public Type SystemType { get; set; }
            public Delegate Factory { get; set; }
            public int Priority { get; set; }

            public SystemRegistration(Type type, Delegate factory, int priority)
            {
                SystemType = type;
                Factory = factory;
                Priority = priority;
            }
        }
    }
}
