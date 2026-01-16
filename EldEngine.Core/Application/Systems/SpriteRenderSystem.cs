using EldEngine.Core.Application.Services;
using EldEngine.Core.Domain;
using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.Core.Infrastructure.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.Core.Application.Systems
{
    public class SpriteRenderSystem : ISystem
    {
        private readonly SpriteAssetManager assetManager;
        private readonly RenderContext renderContext;

        public string Name => nameof(SpriteRenderSystem);

        /// <summary>
        /// Ejecutar después de todas las actualizaciones de lógica
        /// (movimiento, animación, etc)
        /// </summary>
        public int Priority => 999;

        public SpriteRenderSystem(SpriteAssetManager assetManager,
            RenderContext renderContext)
        {
            this.assetManager = assetManager;
            this.renderContext = renderContext;
        }

        public void Execute(World world, float deltaTime)
        {
            try
            {
                // Obtener todas las entidades con Sprite + Transform
                var entities = world
                    .GetEntitiesWith<SpriteComponent, Transform>()
                    .ToList();

                // Ordenar por ZOrder (para renderizado correcto)
                var sortedEntities = entities
                    .OrderBy(e => world.GetComponent<SpriteComponent>(e).ZOrder)
                    .ToList();

                // Renderizar cada entidad
                foreach (var entity in sortedEntities)
                {
                    RenderEntity(world, entity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SpriteRenderSystem] Error: {ex.Message}");
            }
        }

        private void RenderEntity(World world, Entity entity)
        {
            var sprite = world.GetComponent<SpriteComponent>(entity);
            var transform = world.GetComponent<Transform>(entity);

            // Obtener spritesheet
            var spriteSheet = assetManager.GetSpriteSheet(sprite.SpriteId);

            // Determinar frame a renderizar
            Frame frameToRender;

            if (world.HasComponent<AnimationComponent>(entity))
            {
                var animation =
                    world.GetComponent<AnimationComponent>(entity);
                frameToRender = spriteSheet.GetFrame(
                    animation.AnimationName,
                    animation.CurrentFrame);
            }
            else
            {
                // Sin animación, usar primer frame
                frameToRender = spriteSheet.GetFrame("default", 0);
            }

            // Parámetros de renderizado
            var sourceRect = frameToRender.GetSourceRectangle();
            var destRect = new System.Drawing.Rectangle(
                (int)transform.X + frameToRender.Offset.X,
                (int)transform.Y + frameToRender.Offset.Y,
                sprite.Width,
                sprite.Height);

            // Aplicar opciones de renderizado
            var renderOptions = new SpriteRenderOptions
            {
                FlipX = sprite.FlipX,
                FlipY = sprite.FlipY,
                Alpha = sprite.Alpha,
                Tint = sprite.Tint,
                Rotation = transform.Rotation,
                Scale = new System.Drawing.PointF(
                    transform.ScaleX, transform.ScaleY)
            };

            // Renderizar
            renderContext.DrawSprite(spriteSheet.Image, sourceRect,
                destRect, renderOptions);
        }
    }
}
