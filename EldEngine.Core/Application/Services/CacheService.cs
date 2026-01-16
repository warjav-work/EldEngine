using System.Diagnostics;

namespace EldEngine.Core.Application.Services
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 4: Servicio con estado complejo
    /// ═════════════════════════════════════════════════════════════════════════

    public class CacheService : ServiceBase
    {
        public override string Name => "Cache";

        private Dictionary<string, object> _cache = new();
        private Dictionary<string, DateTime> _timestamps = new();
        private float _ttl = 300f;  // 5 minutos

        protected override void OnInitialize()
        {
            Debug.WriteLine($"Cache inicializado con TTL: {_ttl}s");
        }

        protected override void OnDispose()
        {
            _cache.Clear();
            _timestamps.Clear();
        }

        public void Set(string key, object value, float ttl = -1)
        {
            AssertInitialized();
            _cache[key] = value;
            _timestamps[key] = DateTime.Now.AddSeconds(ttl > 0 ? ttl : _ttl);
        }

        public bool TryGet(string key, out object value)
        {
            AssertInitialized();
            value = null;

            // Verificar si la clave existe y no expiró
            if (_cache.TryGetValue(key, out var cachedValue))
            {
                if (_timestamps.TryGetValue(key, out var expiry))
                {
                    if (DateTime.Now < expiry)
                    {
                        value = cachedValue;
                        return true;
                    }
                    else
                    {
                        // Expiró, remover
                        _cache.Remove(key);
                        _timestamps.Remove(key);
                    }
                }
            }

            return false;
        }

        public void Clear()
        {
            AssertInitialized();
            _cache.Clear();
            _timestamps.Clear();
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()} | CachedItems: {_cache.Count} | TTL: {_ttl}s";
        }
    }
}
