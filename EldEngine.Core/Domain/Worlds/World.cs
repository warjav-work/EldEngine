using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Exceptions;
using EldEngine.Core.Domain.Systems;

namespace EldEngine.Core.Domain.Worlds
{
    /// <summary>Centro del sistema ECS. Gestiona entidades, componentes y sistemas</summary>
    public class World : IDisposable
    {
        private readonly ComponentStorage _components = new();
        private readonly List<ISystem> _systems = new();
        private readonly Dictionary<Type, Type> _componentMetadata = new();
        private int _nextEntityId = 1;
        private bool _isRunning = true;

        public event Action<Entity> EntityCreated;
        public event Action<Entity> EntityDestroyed;

        // ==================== ENTITY MANAGEMENT ====================

        public Entity CreateEntity()
        {
            var entity = new Entity(_nextEntityId++);
            EntityCreated?.Invoke(entity);
            return entity;
        }

        public void DestroyEntity(Entity entity)
        {
            if (!entity.IsValid) return;
            EntityDestroyed?.Invoke(entity);
        }

        // ==================== COMPONENT MANAGEMENT ====================

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            ValidateEntity(entity);
            _components.RegisterComponent(entity, component);
        }

        public T GetComponent<T>(Entity entity) where T : IComponent
        {
            ValidateEntity(entity);
            return _components.GetComponent<T>(entity);
        }

        public bool HasComponent<T>(Entity entity) where T : IComponent
        {
            ValidateEntity(entity);
            return _components.HasComponent<T>(entity);
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            ValidateEntity(entity);
            _components.RemoveComponent<T>(entity);
        }

        // ==================== SYSTEM MANAGEMENT ====================

        public void AddSystem(ISystem system)
        {
            _systems.Add(system);
            _systems.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void RemoveSystem(ISystem system) => _systems.Remove(system);

        public T GetSystem<T>() where T : ISystem =>
            _systems.OfType<T>().FirstOrDefault();

        // ==================== QUERY SYSTEMS ====================

        public IEnumerable<Entity> GetEntitiesWith<T1>() where T1 : IComponent =>
            QueryEntities(typeof(T1));

        public IEnumerable<Entity> GetEntitiesWith<T1, T2>()
            where T1 : IComponent where T2 : IComponent =>
            QueryEntities(typeof(T1), typeof(T2));

        public IEnumerable<Entity> GetEntitiesWith<T1, T2, T3>()
            where T1 : IComponent where T2 : IComponent where T3 : IComponent =>
            QueryEntities(typeof(T1), typeof(T2), typeof(T3));

        private IEnumerable<Entity> QueryEntities(params Type[] types) =>
            _components.GetEntitiesWith(types)
                .Where(x => x.Item1.IsValid)
                .Select(x => x.Item1);

        // ==================== EXECUTION ====================

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
                    throw new SystemExecutionException(system.Name, ex);
                }
            }
        }

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
}
