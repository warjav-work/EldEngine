using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using EldEngine.GameTest.Systems;

namespace EldEngine.Tests
{
    public class SystemErrorTests
    {
        [Fact]
        public void PlayerInputSystem_DoesNotThrowException()
        {
            // Arrange
            var world = new World();
            var inputService = new MockInputService();
            var system = new PlayerInputSystem(inputService);

            // Crear múltiples jugadores
            for (int i = 0; i < 10; i++)
            {
                var entity = world.CreateEntity();
                world.AddComponent(entity, new Transform(i * 10, 0));
                world.AddComponent(entity, new Velocity());
                world.AddComponent(entity, new PlayerController());
            }

            // Act & Assert
            var exception = Record.Exception(() => system.Execute(world, 0.016f));
            Assert.Null(exception);
        }
    }

    public class MockInputService : IInputService
    {
        public bool IsKeyPressed(KeyCode key) => false;
        public bool IsKeyDown(KeyCode key) => false;
        public bool IsKeyReleased(KeyCode key) => false;
        public (int, int) GetMousePosition() => (0, 0);
        public bool IsMouseButtonPressed(int button) => false;
        public event Action<KeyCode> OnKeyPressed;
        public event Action<int> OnMousePressed;
    }
}
