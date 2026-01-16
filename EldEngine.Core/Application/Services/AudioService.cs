using System.Diagnostics;

namespace EldEngine.Core.Application.Services
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO 3: Servicio con dependencias
    /// ═════════════════════════════════════════════════════════════════════════

    public class AudioService : ServiceBase
    {
        public override string Name => "Audio";

        private ConfigService _configService;
        private float _volume = 0.8f;

        public AudioService(ConfigService configService)
        {
            _configService = configService ??
                throw new ArgumentNullException(nameof(configService));
        }

        protected override void OnInitialize()
        {
            // Obtener volumen de config
            if (_configService.TryGet("Volume", out float vol))
            {
                _volume = vol;
            }

            Debug.WriteLine($"Audio inicializado con volumen: {_volume}");
        }

        protected override void OnDispose()
        {
            Debug.WriteLine("Audio limpiado");
        }

        public void SetVolume(float volume)
        {
            AssertInitialized();
            _volume = Math.Clamp(volume, 0f, 1f);
        }

        public float GetVolume()
        {
            AssertInitialized();
            return _volume;
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()} | Volume: {_volume:P0}";
        }
    }
}
