using System.Drawing;

namespace EldEngine.Core.Domain.Values
{
    public struct Frame
    {
        /// <summary>
        /// Posición X en la spritesheet
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Posición Y en la spritesheet
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Ancho del frame
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Alto del frame
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Offset para renderizado (para hit-box consistente)
        /// </summary>
        public Point Offset { get; set; }

        /// <summary>
        /// Duración de este frame específico (opcional)
        /// Si es 0, usa la duración del componente
        /// </summary>
        public float Duration { get; set; }

        public Frame(int x, int y, int width, int height,
            Point? offset = null, float duration = 0)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Offset = offset ?? Point.Empty;
            Duration = duration;
        }

        /// <summary>
        /// Obtiene el rectángulo de origen en la spritesheet
        /// </summary>
        public Rectangle GetSourceRectangle() =>
            new Rectangle(X, Y, Width, Height);

        /// <summary>
        /// Obtiene el rectángulo en el que se dibujará
        /// </summary>
        public Rectangle GetDestinationRectangle(int screenX, int screenY,
            int displayWidth, int displayHeight) =>
            new Rectangle(screenX + Offset.X, screenY + Offset.Y,
                displayWidth, displayHeight);


    }
}
