using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Exceptions;
using EldEngine.Core.Domain.Systems;
using System.Diagnostics;

namespace EldEngine.Core.Domain.Worlds
{
    /// <summary>Centro del sistema ECS. Gestiona entidades, componentes y sistemas</summary>
    public class World : IDisposable
    {
        private ComponentStorage _components = new();
        private readonly List<ISystem> _systems = new();
        private readonly Dictionary<Type, Type> _componentMetadata = new();
        private int _nextEntityId = 1;
        private bool _isRunning = true;

        public event Action<Entity> EntityCreated;
        public event Action<Entity> EntityDestroyed;
        public event Action<SystemExecutionException> OnSystemError;

        // ==================== ENTITY MANAGEMENT ====================

        /// <summary>
        /// Crea una nueva entidad con ID único.
        /// </summary>
        public Entity CreateEntity()
        {
            var entity = new Entity(_nextEntityId++);
            EntityCreated?.Invoke(entity);
            return entity;
        }

        /// <summary>
        /// Destruye una entidad y todos sus componentes.
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            if (!entity.IsValid) return;
            EntityDestroyed?.Invoke(entity);
        }

        // ==================== COMPONENT MANAGEMENT ====================

        /// <summary>
        /// Agrega un componente a una entidad.
        /// </summary>
        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            ValidateEntity(entity);
            _components.RegisterComponent(entity, component);
        }

        /// <summary>
        /// Obtiene un componente de una entidad.
        /// </summary>
        public T GetComponent<T>(Entity entity) where T : IComponent
        {
            ValidateEntity(entity);
            return _components.GetComponent<T>(entity);
        }

        /// <summary>
        /// Verifica si una entidad tiene un componente.
        /// </summary>
        public bool HasComponent<T>(Entity entity) where T : IComponent
        {
            ValidateEntity(entity);
            return _components.HasComponent<T>(entity);
        }

        /// <summary>
        /// Remueve un componente de una entidad.
        /// </summary>
        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            ValidateEntity(entity);
            _components.RemoveComponent<T>(entity);
        }

        // ==================== SYSTEM MANAGEMENT ====================

        /// <summary>
        /// Agrega un sistema al mundo.
        /// </summary>
        public void AddSystem(ISystem system)
        {
            _systems.Add(system);
            _systems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            Debug.WriteLine(
                $"✓ Sistema registrado: {system.Name} (Priority: {system.Priority})");
        }

        /// <summary>
        /// Elimina un sistema del mundo.
        /// </summary>
        public void RemoveSystem(ISystem system) => _systems.Remove(system);

        public T GetSystem<T>() where T : ISystem =>
            _systems.OfType<T>().FirstOrDefault();

        // ==================== QUERY SYSTEMS ====================

        /// <summary>
        /// Obtiene todas las entidades con un componente específico.
        /// </summary>
        public IEnumerable<Entity> GetEntitiesWith<T1>() where T1 : IComponent =>
            QueryEntities(typeof(T1));

        /// <summary>
        /// Obtiene todas las entidades con dos componentes específicos.
        /// </summary>
        public IEnumerable<Entity> GetEntitiesWith<T1, T2>()
            where T1 : IComponent where T2 : IComponent =>
            QueryEntities(typeof(T1), typeof(T2));

        /// <summary>
        /// Obtiene todas las entidades con tres componentes específicos.
        /// </summary>
        public IEnumerable<Entity> GetEntitiesWith<T1, T2, T3>()
            where T1 : IComponent where T2 : IComponent where T3 : IComponent =>
            QueryEntities(typeof(T1), typeof(T2), typeof(T3));

        /// <summary>
        /// Query genérico para obtener entidades.
        /// </summary>
        private IEnumerable<Entity> QueryEntities(params Type[] types) =>
            _components.GetEntitiesWith(types)
                .Where(x => x.Item1.IsValid)
                .Select(x => x.Item1);

        // ==================== EXECUTION ====================

        /// <summary>
        /// Actualiza todos los sistemas en orden de prioridad.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (!_isRunning) return;

            foreach (var system in _systems)
            {
                try
                {
                    system.Execute(this, deltaTime);
                }
                catch (Exception ex)
                {
                    var sysEx = new SystemExecutionException(system.Name, ex);
                    OnSystemError?.Invoke(sysEx);
                    throw sysEx;
                }
            }
        }

        /// <summary>
        /// Limpia completamente el mundo (para cambio de escenas).
        /// </summary>
        public void ClearAll()
        {
            System.Diagnostics.Debug.WriteLine("\n🧹 [WORLD] Limpiando completamente...");

            try
            {
                // Notificar destrucción de todas las entidades
                var allEntities = new List<Entity>();
                for (int i = 1; i < _nextEntityId; i++)
                {
                    var entity = new Entity(i);
                    if (entity.IsValid)
                    {
                        allEntities.Add(entity);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"🧹 Destruyendo {allEntities.Count} entidades...");

                // Destruir cada entidad
                foreach (var entity in allEntities)
                {
                    EntityDestroyed?.Invoke(entity);
                }

                // Reinicializar almacenamiento (más eficiente)
                _components = new ComponentStorage();
                _nextEntityId = 1;

                System.Diagnostics.Debug.WriteLine("✓ World completamente limpiado\n");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error limpiando world: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Destruye el mundo y libera recursos.
        /// </summary>
        public void Dispose()
        {
            _isRunning = false;
            _systems.Clear();
            (_components as IDisposable)?.Dispose();
        }        

        private void ValidateEntity(Entity entity)
        {
            if (!entity.IsValid)
                throw new ArgumentException("Entity no es válida", nameof(entity));
        }
    }

    // ==================== ESTADÍSTICAS DEL MUNDO ====================
    /// <summary>
    /// Información estadística del mundo.
    /// </summary>
    public class WorldStats
    {
        public int TotalEntityCount { get; set; }
        public int ActiveSystems { get; set; }
        public List<string> SystemNames { get; set; } = new();

        public override string ToString()
        {
            var systems = string.Join(", ", SystemNames);
            return $"Entidades: {TotalEntityCount} | Sistemas: {ActiveSystems} ({systems})";
        }
    }
}
