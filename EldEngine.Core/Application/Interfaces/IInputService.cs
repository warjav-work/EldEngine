namespace EldEngine.Core.Application.Interfaces
{
    [Flags]
    public enum KeyCode
    {
        Left = 1, Right = 2, Up = 4, Down = 8,
        Space = 16, Enter = 32, Escape = 64,
        A = 128, D = 256, W = 512, S = 1024
    }
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
