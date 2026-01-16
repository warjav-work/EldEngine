using System.Drawing;

namespace EldEngine.Core.Infrastructure.Rendering
{
    public struct SpriteRenderOptions
    {
        public bool FlipX { get; set; }
        public bool FlipY { get; set; }
        public float Alpha { get; set; }
        public Color Tint { get; set; }
        public float Rotation { get; set; }
        public PointF Scale { get; set; }

        public SpriteRenderOptions()
        {
            FlipX = false;
            FlipY = false;
            Alpha = 1.0f;
            Tint = Color.White;
            Rotation = 0f;
            Scale = new PointF(1f, 1f);
        }
    }
}
