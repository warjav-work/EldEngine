using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Rendering
{
    /// <summary>Contexto de rendering usando GDI+ de Windows Forms</summary>
    public class GameRenderContext
    {
        private readonly Form _form;
        private readonly Dictionary<string, Bitmap> _spriteCache = new();

        public GameRenderContext(Form form)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
        }

        public void Render(Graphics g, World world, System.Drawing.Size screenSize)
        {
            try
            {
                // Limpiar pantalla
                g.Clear(Color.FromArgb(30, 30, 40));

                // Renderizar fondo (starfield simple)
                RenderBackground(g, screenSize);

                // Renderizar entidades
                var entities = world.GetEntitiesWith<Transform>();
                foreach (var entity in entities)
                {
                    RenderEntity(g, world, entity);
                }

                // Renderizar UI
                RenderUI(g, screenSize);
            }
            catch (Exception ex)
            {
                g.DrawString($"Error renderizando: {ex.Message}",
                    new Font("Arial", 10),
                    Brushes.Red, 10, 10);
            }
        }

        private void RenderBackground(Graphics g, System.Drawing.Size screenSize)
        {
            // Simple starfield
            var random = new Random(42); // Seed para consistencia
            for (int i = 0; i < 100; i++)
            {
                var x = random.Next(screenSize.Width);
                var y = random.Next(screenSize.Height);
                var brightness = random.Next(100, 255);
                var color = Color.FromArgb(brightness, brightness, brightness);

                g.FillEllipse(new SolidBrush(color), x, y, 1, 1);
            }
        }

        private void RenderEntity(Graphics g, World world, Entity entity)
        {
            var transform = world.GetComponent<Transform>(entity);

            // Determinar color y tamaño según tipo
            Color color = Color.White;
            float size = 16f;

            if (world.HasComponent<PlayerController>(entity))
            {
                color = Color.Lime;
                size = 20f;
            }
            else if (world.HasComponent<NpcBehavior>(entity))
            {
                color = Color.Cyan;
                size = 16f;
            }
            else if (world.HasComponent<CombatStats>(entity))
            {
                color = Color.Red;
                size = 14f;
            }

            // Renderizar como círculo (placeholder)
            var rect = new RectangleF(
                transform.X - size / 2,
                transform.Y - size / 2,
                size,
                size);

            g.FillEllipse(new SolidBrush(color), rect);
            g.DrawEllipse(new Pen(Color.White, 2), rect);

            // Renderizar nombre si es NPC
            if (world.HasComponent<NpcBehavior>(entity))
            {
                var npc = world.GetComponent<NpcBehavior>(entity);
                g.DrawString(npc.NpcName,
                    new Font("Arial", 8),
                    Brushes.Cyan,
                    transform.X - 20,
                    transform.Y - 25);
            }

            // Renderizar salud si tiene
            if (world.HasComponent<CombatStats>(entity))
            {
                var stats = world.GetComponent<CombatStats>(entity);
                var healthPercent = (float)stats.CurrentHealth / stats.MaxHealth;

                // Barra de vida
                var healthBarWidth = 30;
                var healthBarHeight = 4;
                var healthBarX = transform.X - healthBarWidth / 2;
                var healthBarY = transform.Y + size / 2 + 5;

                // Fondo (rojo)
                g.FillRectangle(Brushes.DarkRed,
                    healthBarX, healthBarY, healthBarWidth, healthBarHeight);

                // Vida (verde)
                g.FillRectangle(Brushes.LimeGreen,
                    healthBarX, healthBarY,
                    healthBarWidth * healthPercent, healthBarHeight);

                // Borde
                g.DrawRectangle(Pens.White,
                    healthBarX, healthBarY, healthBarWidth, healthBarHeight);
            }
        }

        private void RenderUI(Graphics g, System.Drawing.Size screenSize)
        {
            // Panel de información (esquina superior derecha)
            var uiFont = new Font("Consolas", 10, FontStyle.Bold);
            var textBrush = Brushes.LimeGreen;
            var padding = 10;

            g.DrawString("ELD ENGINE v1.0", uiFont, Brushes.White,
                screenSize.Width - 250, padding);

            g.DrawString("CONTROLES:", new Font("Consolas", 9), Brushes.Cyan,
                screenSize.Width - 250, padding + 25);

            g.DrawString("WASD/Flechas: Mover", uiFont, textBrush,
                screenSize.Width - 250, padding + 45);

            g.DrawString("ESPACIO: Dash", uiFont, textBrush,
                screenSize.Width - 250, padding + 65);

            g.DrawString("ESC: Salir", uiFont, textBrush,
                screenSize.Width - 250, padding + 85);
        }

        public void LoadSprite(string path)
        {
            if (!_spriteCache.ContainsKey(path))
            {
                try
                {
                    _spriteCache[path] = new Bitmap(path);
                }
                catch
                {
                    // Sprite no encontrado, usar placeholder
                }
            }
        }
    }
}
