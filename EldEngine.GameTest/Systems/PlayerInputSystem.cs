using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Procesa input del jugador y actualiza su velocidad.
    /// </summary>
    public class PlayerInputSystem : ISystem
    {
        private readonly IInputService _inputService;

        public string Name => nameof(PlayerInputSystem);
        public int Priority => 10; // Primero: input -> movimiento -> render

        public PlayerInputSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void Execute(World world, float deltaTime)
        {
            // Obtener todas las entidades jugador
            var players = world.GetEntitiesWith<PlayerController, Velocity>().ToList();

            foreach (var player in players)
            {
                if (!player.IsValid) continue;

                var controller = world.GetComponent<PlayerController>(player);
                var velocity = world.GetComponent<Velocity>(player);
                // Procesar input
                velocity.X = 0;
                velocity.Y = 0;

                // Procesar input horizontal
                if (_inputService.IsKeyDown(KeyCode.A) || _inputService.IsKeyDown(KeyCode.Left))
                    velocity.X = -controller.MoveSpeed;
                if (_inputService.IsKeyDown(KeyCode.D) || _inputService.IsKeyDown(KeyCode.Right))
                    velocity.X = controller.MoveSpeed;

                // Procesar input vertical
                if (_inputService.IsKeyDown(KeyCode.W) || _inputService.IsKeyDown(KeyCode.Up))
                    velocity.Y = -controller.MoveSpeed;
                if (_inputService.IsKeyDown(KeyCode.S) || _inputService.IsKeyDown(KeyCode.Down))
                    velocity.Y = controller.MoveSpeed;

                // Procesar dash (cooldown)
                if (_inputService.IsKeyPressed(KeyCode.Space) && controller.CanDash)
                {
                    controller.CanDash = false;
                    controller.DashCooldown = 1f;
                    velocity.X *= controller.DashSpeed / controller.MoveSpeed;
                    velocity.Y *= controller.DashSpeed / controller.MoveSpeed;
                }

                controller.DashCooldown -= deltaTime;
                if (controller.DashCooldown <= 0)
                    controller.CanDash = true;

                // Actualizar componentes
                world.RemoveComponent<Velocity>(player);
                world.AddComponent(player, velocity);

                world.RemoveComponent<PlayerController>(player);
                world.AddComponent(player, controller);
            }
        }
    }
}
