using EldEngine.Core.Application.Interfaces;

namespace EldEngine.Core.Application.Services
{
    public class InputManager : IInputService
    {
        private readonly HashSet<KeyCode> _pressedKeys = new();
        private readonly HashSet<KeyCode> _downKeys = new();
        private readonly HashSet<KeyCode> _releasedKeys = new();
        private (int X, int Y) _mousePosition;

        public event Action<KeyCode> OnKeyPressed;
        public event Action<int> OnMousePressed;

        public bool IsKeyPressed(KeyCode key) => _pressedKeys.Contains(key);
        public bool IsKeyDown(KeyCode key) => _downKeys.Contains(key);
        public bool IsKeyReleased(KeyCode key) => _releasedKeys.Contains(key);
        public (int X, int Y) GetMousePosition() => _mousePosition;
        public bool IsMouseButtonPressed(int button) => false; // Implementar con input real

        public void Update()
        {
            _pressedKeys.Clear();
            _releasedKeys.Clear();
            // Actualizar estados desde el sistema de input real
        }

        internal void ProcessKeyDown(KeyCode key)
        {
            if (!_downKeys.Contains(key))
            {
                _pressedKeys.Add(key);
                OnKeyPressed?.Invoke(key);
            }
            _downKeys.Add(key);
        }

        internal void ProcessKeyUp(KeyCode key)
        {
            _downKeys.Remove(key);
            _releasedKeys.Add(key);
        }

        internal void SetMousePosition(int x, int y) => _mousePosition = (x, y);
    }
}
