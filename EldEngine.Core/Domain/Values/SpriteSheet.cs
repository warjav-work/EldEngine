using System.Drawing;

namespace EldEngine.Core.Domain.Values
{
    public class SpriteSheet
    {
        /// <summary>
        /// ID único de la spritesheet
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Imagen cargada en memoria
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Ancho de cada tile/frame
        /// </summary>
        public int TileWidth { get; set; }

        /// <summary>
        /// Alto de cada tile/frame
        /// </summary>
        public int TileHeight { get; set; }

        /// <summary>
        /// Colecciones de animaciones por nombre
        /// Key: "idle", "walk", "jump"
        /// Value: List de Frames
        /// </summary>
        private Dictionary<string, List<Frame>> animations;

        public SpriteSheet(string id, Bitmap image, int tileWidth,
            int tileHeight)
        {
            Id = id;
            Image = image;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            animations = new Dictionary<string, List<Frame>>();
        }

        /// <summary>
        /// Agregar animación a la spritesheet
        /// </summary>
        public void AddAnimation(string name, List<Frame> frames)
        {
            animations[name] = frames;
        }

        /// <summary>
        /// Obtener frames de una animación
        /// </summary>
        public List<Frame> GetAnimation(string name)
        {
            if (!animations.ContainsKey(name))
                throw new KeyNotFoundException(
                    $"Animación '{name}' no encontrada en {Id}");

            return animations[name];
        }

        /// <summary>
        /// Obtener frame específico
        /// </summary>
        public Frame GetFrame(string animationName, int frameIndex)
        {
            var frames = GetAnimation(animationName);
            return frames[frameIndex % frames.Count];
        }

        /// <summary>
        /// Liberar recursos de memoria
        /// </summary>
        public void Dispose()
        {
            Image?.Dispose();
            animations.Clear();
        }

        /// <summary>
        /// Generar spritesheet automático en grid
        /// Útil para spritesheets regulares (todos los frames mismo tamaño)
        /// </summary>
        public static SpriteSheet CreateFromGrid(string id,
            Bitmap image, int tileWidth, int tileHeight)
        {
            var sheet = new SpriteSheet(id, image, tileWidth, tileHeight);

            // Calcular cuántos tiles caben en la imagen
            int tilesX = image.Width / tileWidth;
            int tilesY = image.Height / tileHeight;

            // Generar animación "default" con todos los frames
            var frames = new List<Frame>();
            for (int y = 0; y < tilesY; y++)
            {
                for (int x = 0; x < tilesX; x++)
                {
                    frames.Add(new Frame(
                        x * tileWidth,
                        y * tileHeight,
                        tileWidth,
                        tileHeight
                    ));
                }
            }

            sheet.AddAnimation("default", frames);
            return sheet;
        }
    }
}
