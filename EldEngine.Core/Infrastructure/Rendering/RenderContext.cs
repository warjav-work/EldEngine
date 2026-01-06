using EldEngine.Core.Application.Interfaces;

namespace EldEngine.Core.Infrastructure.Rendering
{
    public class RenderContext : IRenderService
    {
        private int _width;
        private int _height;
        private bool _isRunning = true;

        public bool IsRunning => _isRunning;

        public void Initialize(int width, int height, string title)
        {
            _width = width;
            _height = height;
            // Inicializar contexto gráfico (SharpDX/MonoGame)
        }

        public void BeginFrame()
        {
            // Preparar frame de renderizado
        }

        public void EndFrame()
        {
            // Presentar frame
        }

        public void Clear(float r = 0, float g = 0, float b = 0)
        {
            // Limpiar pantalla con color especificado
        }

        public void DrawSprite(float x, float y, string assetPath, float rotation = 0)
        {
            // Renderizar sprite
        }

        public void DrawRectangle(float x, float y, float width, float height, uint color)
        {
            // Renderizar rectángulo
        }

        public void DrawText(string text, float x, float y, float size, uint color)
        {
            // Renderizar texto
        }

        public void Dispose()
        {
            _isRunning = false;
        }
    }
}
