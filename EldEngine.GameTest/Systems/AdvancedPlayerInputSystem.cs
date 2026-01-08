using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Procesa input del jugador y actualiza su velocidad.
    /// Input mejorado que respeta collisiones
    /// </summary>
    public class AdvancedPlayerInputSystem : ISystem
    {
        private readonly IInputService _inputService;
        private CollisionSystem _collisionSystem;

        public string Name => nameof(AdvancedPlayerInputSystem);
        public int Priority => 10;

        public AdvancedPlayerInputSystem(IInputService inputService, CollisionSystem collisionSystem)
        {
            _inputService = inputService;
            _collisionSystem = collisionSystem;
        }

        public void Execute(World world, float deltaTime)
        {
            var players = world.GetEntitiesWith<PlayerController, Velocity>().ToList();

            foreach (var player in players)
            {
                var controller = world.GetComponent<PlayerController>(player);
                var velocity = world.GetComponent<Velocity>(player);

                velocity.X = 0;
                velocity.Y = 0;

                // Input de Movimiento
                if (_inputService.IsKeyDown(KeyCode.W) || _inputService.IsKeyDown(KeyCode.Up))
                    velocity.Y = -controller.MoveSpeed;
                if (_inputService.IsKeyDown(KeyCode.S) || _inputService.IsKeyDown(KeyCode.Down))
                    velocity.Y = controller.MoveSpeed;
                if (_inputService.IsKeyDown(KeyCode.A) || _inputService.IsKeyDown(KeyCode.Left))
                    velocity.X = -controller.MoveSpeed;
                if (_inputService.IsKeyDown(KeyCode.D) || _inputService.IsKeyDown(KeyCode.Right))
                    velocity.X = controller.MoveSpeed;

                // Correr ligeramente
                if (_inputService.IsKeyPressed(KeyCode.Space))
                {
                    if (controller.CanDash)
                    {
                        controller.CanDash = false;
                        velocity.X *= 3f;
                        velocity.Y *= 3f;
                    }
                }

                controller.DashCooldown -= deltaTime;
                if (controller.DashCooldown <= 0)
                {
                    controller.CanDash = true;
                }

                // Actualizar
                world.RemoveComponent<Velocity>(player);
                world.AddComponent(player, velocity);

                world.RemoveComponent<PlayerController>(player);
                world.AddComponent(player, controller);
            }
        }
    }
}
