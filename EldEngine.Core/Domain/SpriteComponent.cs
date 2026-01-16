using EldEngine.Core.Domain.Entities;

namespace EldEngine.Core.Domain
{
    public struct SpriteComponent : IComponent
    {
        /// <summary>
        /// Identificador único del sprite en el AssetManager
        /// Ejemplo: "player_idle", "enemy_walk", "tileset_grass"
        /// </summary>
        public string SpriteId { get; set; }

        /// <summary>
        /// Tamaño de renderizado en píxeles
        /// </summary>
        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// Opacidad: 0.0 (invisible) a 1.0 (opaco)
        /// </summary>
        public float Alpha { get; set; }

        /// <summary>
        /// Voltear sprite horizontalmente
        /// </summary>
        public bool FlipX { get; set; }

        /// <summary>
        /// Voltear sprite verticalmente
        /// </summary>
        public bool FlipY { get; set; }

        /// <summary>
        /// Capa de renderizado (0-100)
        /// Mayor número = renderizado más adelante
        /// </summary>
        public int ZOrder { get; set; }

        /// <summary>
        /// Tint (coloreo) del sprite
        /// Color.White = sin cambio de color
        /// </summary>
        public System.Drawing.Color Tint { get; set; }

        public SpriteComponent(string spriteId, int width, int height)
        {
            SpriteId = spriteId;
            Width = width;
            Height = height;
            Alpha = 1.0f;
            FlipX = false;
            FlipY = false;
            ZOrder = 0;
            Tint = System.Drawing.Color.White;
        }

        static int IComponent.GetComponentTypeId() =>
            typeof(SpriteComponent).GetHashCode();
    }
}
