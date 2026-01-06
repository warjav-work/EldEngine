namespace EldEngine.Core.Domain.Entities
{
    // Sparse Set Pattern para O(1) lookup y mejor cache locality.
    class SparseSet
    {
        private Dictionary<int, int> _sparse = new();
        private Dictionary<int, object> _dense = new();
        private int _size = 0;

        public void Add(int entityId, object component)
        {
            if (_sparse.ContainsKey(entityId))
                _sparse[entityId] = _dense.Count;
            else
                _sparse.Add(entityId, _dense.Count);

            _dense[_sparse[entityId]] = component;
            _size++;
        }

        public object Get(int entityId) =>
            _sparse.ContainsKey(entityId) ? _dense[_sparse[entityId]] : null;

        public bool Has(int entityId) => _sparse.ContainsKey(entityId);

        public void Remove(int entityId)
        {
            if (!_sparse.ContainsKey(entityId)) return;

            var index = _sparse[entityId];
            _sparse.Remove(entityId);
            _size--;
        }

        public IEnumerable<int> GetEntities() => _sparse.Keys;
    }
}
