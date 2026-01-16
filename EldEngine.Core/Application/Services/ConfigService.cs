namespace EldEngine.Core.Application.Services
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 2: Servicio con recursos
    /// ═════════════════════════════════════════════════════════════════════════

    public class ConfigService : ServiceBase
    {
        public override string Name => "Config";
        public override string Version => "2.0";

        private Dictionary<string, object> _config = new();

        protected override void OnInitialize()
        {
            // Cargar configuración por defecto
            _config["GameTitle"] = "Elder: Chronicles of the Silver Grove";
            _config["ScreenWidth"] = 1024;
            _config["ScreenHeight"] = 768;
            _config["MaxFPS"] = 60;
            _config["Volume"] = 0.8f;
        }

        protected override void OnDispose()
        {
            _config.Clear();
        }

        public void Set(string key, object value)
        {
            AssertInitialized();
            _config[key] = value;
        }

        public T Get<T>(string key)
        {
            AssertInitialized();
            return (T)_config[key];
        }

        public bool TryGet<T>(string key, out T value)
        {
            AssertInitialized();
            value = default;

            if (_config.TryGetValue(key, out var obj) && obj is T typed)
            {
                value = typed;
                return true;
            }

            return false;
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()} | ConfigKeys: {_config.Count}";
        }
    }
}
