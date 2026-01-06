using EldEngine.Core.Application.Interfaces;

namespace EldEngine.GameTest.Inputs
{
    /// <summary>Implementación de IInputService usando Windows Forms</summary>
    public class WindowsFormsInputService : IInputService
    {
        private readonly Form _gameForm;
        private readonly Dictionary<KeyCode, bool> _currentKeyState = new();
        private readonly Dictionary<KeyCode, bool> _previousKeyState = new();
        private (int X, int Y) _mousePosition = (0, 0);

        public event Action<KeyCode> OnKeyPressed;
        public event Action<int> OnMousePressed;

        public WindowsFormsInputService(Form gameForm)
        {
            _gameForm = gameForm ?? throw new ArgumentNullException(nameof(gameForm));

            // Inicializar todas las teclas
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                _currentKeyState[key] = false;
                _previousKeyState[key] = false;
            }

            // Eventos
            _gameForm.KeyDown += GameForm_KeyDown;
            _gameForm.KeyUp += GameForm_KeyUp;
            _gameForm.MouseMove += GameForm_MouseMove;
            _gameForm.MouseDown += GameForm_MouseDown;

            System.Diagnostics.Debug.WriteLine("✓ Input Service inicializado");
        }

        public bool IsKeyPressed(KeyCode key)
        {
            var isCurrentlyPressed = _currentKeyState.GetValueOrDefault(key, false);
            var wasPreviouslyPressed = _previousKeyState.GetValueOrDefault(key, false);
            return isCurrentlyPressed && !wasPreviouslyPressed;
        }

        public bool IsKeyDown(KeyCode key)
        {
            return _currentKeyState.GetValueOrDefault(key, false);
        }

        public bool IsKeyReleased(KeyCode key)
        {
            var isCurrentlyPressed = _currentKeyState.GetValueOrDefault(key, false);
            var wasPreviouslyPressed = _previousKeyState.GetValueOrDefault(key, false);
            return !isCurrentlyPressed && wasPreviouslyPressed;
        }

        public (int X, int Y) GetMousePosition() => _mousePosition;
        public bool IsMouseButtonPressed(int button) => false;

        public void Update()
        {
            foreach (var kvp in new Dictionary<KeyCode, bool>(_currentKeyState))
            {
                _previousKeyState[kvp.Key] = kvp.Value;
            }
        }

        // ==================== EVENT HANDLERS ====================

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            var keyCode = TranslateKey(e.KeyCode);
            _currentKeyState[keyCode] = true;  // ← PROCESAR TODAS
            OnKeyPressed?.Invoke(keyCode);

            System.Diagnostics.Debug.WriteLine($"KeyDown: {e.KeyCode} → {keyCode}");
            e.Handled = true;
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            var keyCode = TranslateKey(e.KeyCode);
            _currentKeyState[keyCode] = false;
            System.Diagnostics.Debug.WriteLine($"KeyUp: {e.KeyCode}");
            e.Handled = true;
        }

        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = (e.X, e.Y);
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            OnMousePressed?.Invoke((int)e.Button);
        }

        // ==================== KEY TRANSLATION ====================

        private KeyCode TranslateKey(Keys key)
        {
            return key switch
            {
                Keys.W => KeyCode.W,
                Keys.A => KeyCode.A,
                Keys.S => KeyCode.S,
                Keys.D => KeyCode.D,
                Keys.Left => KeyCode.Left,
                Keys.Right => KeyCode.Right,
                Keys.Up => KeyCode.Up,
                Keys.Down => KeyCode.Down,
                Keys.Space => KeyCode.Space,
                Keys.Return => KeyCode.Enter,
                Keys.Escape => KeyCode.Escape,
                _ => KeyCode.A  // Default
            };
        }
    }
}
