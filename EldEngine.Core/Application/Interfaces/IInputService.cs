namespace EldEngine.Core.Application.Interfaces
{
    public interface IInputService
    {
        bool IsKeyPressed(KeyCode key);
        bool IsKeyDown(KeyCode key);
        bool IsKeyReleased(KeyCode key);

        (int X, int Y) GetMousePosition();
        bool IsMouseButtonPressed(int button);

        event Action<KeyCode> OnKeyPressed;
        event Action<int> OnMousePressed;
    }
}
