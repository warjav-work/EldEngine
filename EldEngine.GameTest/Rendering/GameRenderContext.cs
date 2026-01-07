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
                var allEntities = world.GetEntitiesWith<Transform>().ToList();

                // Separar por tipo para controlar orden de renderizado
                var walls = new List<Entity>();
                var obstacles = new List<Entity>();
                var npcs = new List<Entity>();
                var enemies = new List<Entity>();
                var players = new List<Entity>();
                var other = new List<Entity>();

                foreach (var entity in allEntities)
                {
                    if (!entity.IsValid) continue;

                    if (world.HasComponent<PlayerController>(entity))
                        players.Add(entity);
                    else if (world.HasComponent<ObstacleComponent>(entity))
                    {
                        var obs = world.GetComponent<ObstacleComponent>(entity);
                        if (obs.ObstacleType == "wall")
                            walls.Add(entity);
                        else
                            obstacles.Add(entity);
                    }
                    else if (world.HasComponent<EnemyAIComponent>(entity))
                        enemies.Add(entity);
                    else if (world.HasComponent<NpcBehavior>(entity))
                        npcs.Add(entity);
                    else
                        other.Add(entity);
                }


                // Renderizar en orden (back to front)
                foreach (var entity in walls)
                    RenderEntity(g, world, entity, Color.Gray);

                foreach (var entity in obstacles)
                    RenderEntity(g, world, entity, Color.SaddleBrown);

                foreach (var entity in enemies)
                    RenderEntity(g, world, entity, Color.Red);

                foreach (var entity in npcs)
                    RenderEntity(g, world, entity, Color.Cyan);

                foreach (var entity in players)
                    RenderEntity(g, world, entity, Color.Lime);

                foreach (var entity in other)
                    RenderEntity(g, world, entity, Color.White);

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
        /*
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
        }*/

        /// <summary>Renderiza una entidad individual con su collider si existe</summary>
        private void RenderEntity(Graphics g, World world, Entity entity, Color baseColor)
        {
            var transform = world.GetComponent<Transform>(entity);

            // Obtener tamaño del collider si existe
            float width = 16;
            float height = 16;
            string label = "";

            if (world.HasComponent<ColliderComponent>(entity))
            {
                var collider = world.GetComponent<ColliderComponent>(entity);
                width = collider.Width;
                height = collider.Height;
                label = collider.Tag;
            }

            // Calcular rectángulo
            var rect = new RectangleF(
                transform.X - width / 2,
                transform.Y - height / 2,
                width,
                height);

            // Renderizar según tipo
            if (world.HasComponent<ObstacleComponent>(entity))
            {
                RenderObstacle(g, world, entity, rect, baseColor);
            }
            else if (world.HasComponent<EnemyAIComponent>(entity))
            {
                RenderEnemy(g, world, entity, rect, baseColor);
            }
            else if (world.HasComponent<PlayerController>(entity))
            {
                RenderPlayer(g, world, entity, rect, baseColor);
            }
            else
            {
                // Entidad genérica
                g.FillRectangle(new SolidBrush(baseColor), rect);
                g.DrawRectangle(new Pen(Color.White, 1), rect);
            }

            // Renderizar salud si la tiene
            if (world.HasComponent<CombatStats>(entity))
            {
                RenderHealthBar(g, world, entity, rect);
            }
        }
        /// <summary>Renderiza obstáculos (árboles, rocas, paredes)</summary>
        private void RenderObstacle(Graphics g, World world, Entity entity, RectangleF rect, Color baseColor)
        {
            var obstacle = world.GetComponent<ObstacleComponent>(entity);

            // Dibujar rectángulo del obstáculo
            g.FillRectangle(new SolidBrush(baseColor), rect);

            // Borde
            var borderColor = Color.FromArgb(100, 0, 0, 0);
            g.DrawRectangle(new Pen(borderColor, 2), rect);

            // Mostrar tipo de obstáculo
            var typeChar = obstacle.ObstacleType switch
            {
                "wall" => "W",
                "tree" => "🌳",
                "rock" => "⬜",
                "door" => "D",
                _ => "?"
            };

            g.DrawString(typeChar,
                new Font("Arial", 10, FontStyle.Bold),
                Brushes.White,
                rect.X + rect.Width / 2 - 5,
                rect.Y + rect.Height / 2 - 7);
        }
        /// <summary>Renderiza enemigos con IA visible</summary>
        private void RenderEnemy(Graphics g, World world, Entity entity, RectangleF rect, Color baseColor)
        {
            var ai = world.GetComponent<EnemyAIComponent>(entity);

            // Círculo para enemigos
            g.FillEllipse(new SolidBrush(baseColor), rect);

            // Borde según estado de IA
            Color borderColor = ai.CanSeePlayer ? Color.Orange : Color.White;
            float borderWidth = ai.CanSeePlayer ? 3 : 1.5f;

            g.DrawEllipse(new Pen(borderColor, borderWidth), rect);

            // Mostrar tipo de IA
            var aiChar = ai.AIType switch
            {
                "patrol" => "P",
                "follow" => "F",
                "aggressive" => "A",
                _ => "?"
            };

            g.DrawString(aiChar,
                new Font("Arial", 12, FontStyle.Bold),
                Brushes.White,
                rect.X + rect.Width / 2 - 5,
                rect.Y + rect.Height / 2 - 7);

            // Mostrar rango de detección si ve al jugador
            if (ai.CanSeePlayer)
            {
                g.DrawEllipse(new Pen(Color.FromArgb(50, Color.Orange), 1),
                    rect.X - ai.DetectionRange / 2,
                    rect.Y - ai.DetectionRange / 2,
                    ai.DetectionRange,
                    ai.DetectionRange);
            }
        }

        /// <summary>Renderiza el jugador</summary>
        private void RenderPlayer(Graphics g, World world, Entity entity, RectangleF rect, Color baseColor)
        {
            var controller = world.GetComponent<PlayerController>(entity);

            // Cuadrado para jugador
            g.FillRectangle(new SolidBrush(baseColor), rect);

            // Borde destacado
            g.DrawRectangle(new Pen(Color.Yellow, 3), rect);

            // Mostrar velocidad del dash
            if (!controller.CanDash)
            {
                var dashPercent = (1f - controller.DashCooldown) * 100;
                g.DrawString($"D:{dashPercent:F0}%",
                    new Font("Arial", 8),
                    Brushes.Yellow,
                    rect.X + 2,
                    rect.Y - 15);
            }
        }

        /// <summary>Renderiza barra de vida</summary>
        private void RenderHealthBar(Graphics g, World world, Entity entity, RectangleF rect)
        {
            var stats = world.GetComponent<CombatStats>(entity);

            if (stats.MaxHealth <= 0) return;

            float healthPercent = (float)stats.CurrentHealth / stats.MaxHealth;
            float barWidth = rect.Width;
            float barHeight = 3;
            float barX = rect.X;
            float barY = rect.Y - 8;

            // Fondo (rojo)
            g.FillRectangle(Brushes.DarkRed, barX, barY, barWidth, barHeight);

            // Vida (verde)
            var healthColor = healthPercent > 0.5f ? Color.LimeGreen :
                             healthPercent > 0.25f ? Color.Yellow : Color.Red;

            g.FillRectangle(new SolidBrush(healthColor), barX, barY, barWidth * healthPercent, barHeight);

            // Borde
            g.DrawRectangle(new Pen(Color.White, 0.5f), barX, barY, barWidth, barHeight);

            // Mostrar números
            g.DrawString($"{stats.CurrentHealth}/{stats.MaxHealth}",
                new Font("Arial", 7),
                Brushes.White,
                barX + 2,
                barY - 12);
        }

        /// <summary>Renderiza fondo decorativo</summary>
        private void RenderBackground(Graphics g, System.Drawing.Size screenSize)
        {
            // Patrón de cuadrícula suave
            float gridSize = 50;
            var gridPen = new Pen(Color.FromArgb(20, 100, 100, 100), 0.5f);

            for (float x = 0; x < screenSize.Width; x += gridSize)
            {
                g.DrawLine(gridPen, (int)x, 0, (int)x, screenSize.Height);
            }

            for (float y = 0; y < screenSize.Height; y += gridSize)
            {
                g.DrawLine(gridPen, 0, (int)y, screenSize.Width, (int)y);
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
