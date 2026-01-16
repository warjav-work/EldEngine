using EldEngine.Core.Application.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace EldEngine.Core.Infrastructure.Rendering
{
    public class RenderContext : IRenderService
    {
        private int _width;
        private int _height;
        private bool _isRunning = true;
        private Graphics graphics;

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

        /// <summary>
        /// Renderizar sprite con opciones avanzadas
        /// </summary>
        public void DrawSprite(Image sourceImage, Rectangle sourceRect,
            Rectangle destRect, SpriteRenderOptions options)
        {
            if (graphics == null) return;

            try
            {
                // Crear atributos de imagen para aplicar opciones
                var attributes = new ImageAttributes();

                // Aplicar Tint (coloreo)
                if (options.Tint != Color.White)
                {
                    var colorMatrix = new ColorMatrix(new float[][]
                    {
                    new float[] { options.Tint.R / 255f, 0, 0, 0, 0 },
                    new float[] { 0, options.Tint.G / 255f, 0, 0, 0 },
                    new float[] { 0, 0, options.Tint.B / 255f, 0, 0 },
                    new float[] { 0, 0, 0, options.Alpha, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                    });
                    attributes.SetColorMatrix(colorMatrix);
                }
                else if (options.Alpha < 1.0f)
                {
                    // Solo aplicar alpha
                    var colorMatrix = new ColorMatrix(new float[][]
                    {
                    new float[] { 1, 0, 0, 0, 0 },
                    new float[] { 0, 1, 0, 0, 0 },
                    new float[] { 0, 0, 1, 0, 0 },
                    new float[] { 0, 0, 0, options.Alpha, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                    });
                    attributes.SetColorMatrix(colorMatrix);
                }

                // Aplicar flip si es necesario
                Image imageToRender = sourceImage;
                if (options.FlipX || options.FlipY)
                {
                    imageToRender = new Bitmap(sourceImage);
                    if (options.FlipX)
                        imageToRender.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    if (options.FlipY)
                        imageToRender.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }

                // Renderizar
                /*graphics.DrawImage(
                    imageToRender,
                    new Rectangle[] { destRect },
                    sourceRect,
                    GraphicsUnit.Pixel,
                    attributes);*/

                graphics.DrawImage(imageToRender, destRect, sourceRect.X, sourceRect.Y,
                    sourceRect.Width, sourceRect.Height,
                    GraphicsUnit.Pixel, attributes);

                // Limpiar bitmap temporal si se creó
                if (imageToRender != sourceImage)
                    imageToRender.Dispose();

                attributes?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Error al renderizar sprite: {ex.Message}");
            }
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
