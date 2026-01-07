namespace EldEngine.Core.Domain.Entities
{
    /// <summary>
    /// Almacenamiento sparse set de componentes para máximo rendimiento.
    /// </summary>
    public class ComponentStorage
    {
        private readonly Dictionary<Type, SparseSet> _storages = new();

        public void RegisterComponent<T>(Entity entity, T component) where T : IComponent
        {
            var type = typeof(T);
            if (!_storages.ContainsKey(type))
                _storages[type] = new SparseSet();

            _storages[type].Add(entity.Id, component);
        }

        public T GetComponent<T>(Entity entity) where T : IComponent
        {
            var type = typeof(T);
            return _storages.ContainsKey(type)
                ? (T)_storages[type].Get(entity.Id)
                : throw new InvalidOperationException($"{entity} no tiene componente {type.Name}");
        }

        public bool HasComponent<T>(Entity entity) where T : IComponent
        {
            var type = typeof(T);
            return _storages.ContainsKey(type) && _storages[type].Has(entity.Id);
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent
        {
            var type = typeof(T);
            if (_storages.ContainsKey(type))
                _storages[type].Remove(entity.Id);
        }

        public IEnumerable<(Entity, object)> GetEntitiesWith(params Type[] componentTypes)
        {
            if (componentTypes.Length == 0)
                return Enumerable.Empty<(Entity, object)>();

            var baseSparseSet = _storages.FirstOrDefault(
                x => componentTypes.Contains(x.Key)).Value;

            if (baseSparseSet == null)
                return Enumerable.Empty<(Entity, object)>();

            var validEntities = baseSparseSet.GetEntities()
                .Where(id => componentTypes.All(type =>
                    _storages.ContainsKey(type) && _storages[type].Has(id)))
                .Select(id => (new Entity(id), (object)null));

            return validEntities;
        }
    }
}
