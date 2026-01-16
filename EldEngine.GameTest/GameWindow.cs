using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Application.Services;
using EldEngine.Core.Application.Systems;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.GameTest.GameStates;
using EldEngine.GameTest.Inputs;
using EldEngine.GameTest.Rendering;
using EldEngine.GameTest.Scenes;
using EldEngine.GameTest.Systems;
using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

namespace EldEngine.GameTest
{
    /// <summary>
    /// Ventana principal del juego con integración del motor ECS.
    /// </summary>
    public partial class GameWindow : Form
    {
        // ==================== SERVICIOS PRINCIPALES ====================
        private IGameService _gameService;
        private ISceneService _sceneService;
        private IInputService _inputService;

        // ==================== MANAGERS ====================
        private GameManager _gameManager;
        private LevelManager _levelManager;

        // ==================== SISTEMAS ====================
        private CollisionSystem _collisionSystem;
        private EnemyAISystem _enemyAISystem;

        // ==================== RENDERING ====================
        private GameRenderContext _renderContext;
        private Stopwatch _gameTimer;
        private float _deltaTime;

        // ==================== UI ====================     
        private Label _statusLabel;
        private Label _scoreLabel;
        private Label _levelLabel;
        private Label _stateLabel;
        private Label _messageLabel;
        private Timer _messageTimer;

        private Timer _gameLoopTimer;
        private Timer _uiUpdateTimer;

        // Flag para evitar recursión
        private bool _isClosing = false;
        public GameWindow()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Focus();
            InitializeGame();
        }

        protected override void OnClosed(EventArgs e)
        {
            // Solo ejecutar una vez
            if (_isClosing)
            {
                base.OnClosed(e);
                return;
            }

            _isClosing = true;

            Debug.WriteLine("\n╔════════════════════════════════════════════════════════╗");
            Debug.WriteLine("║  CERRANDO APLICACIÓN                                   ║");
            Debug.WriteLine("║  Limpiando recursos...                                 ║");
            Debug.WriteLine("╚════════════════════════════════════════════════════════╝\n");

            try
            {
                // Guardar datos si es necesario
                if (_gameManager != null)
                {
                    _gameManager.SaveData("LastExitTime", DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error guardando datos: {ex.Message}");
            }

            try
            {
                // Detener todos los timers
                _messageTimer?.Stop();
                _gameLoopTimer?.Stop();
                _uiUpdateTimer?.Stop();

                // Detener stopwatch
                _gameTimer?.Stop();

                // Limpiar servicios
                _gameService?.Dispose();

                // Limpiar UI
                this.Controls.Clear();

                Debug.WriteLine("✓ Recursos liberados correctamente\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Error durante limpieza: {ex.Message}");
            }

            base.OnClosed(e);
        }

        /// <summary>
        /// Inicializa el motor ECS y la ventana.
        /// </summary>
        private void InitializeGame()
        {
            try
            {
                // Configurar ventana
                this.Text = "Elder: Chronicles of the Silver Grove";
                this.Size = new System.Drawing.Size(1024, 768);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.DoubleBuffered = true;
                this.BackColor = System.Drawing.Color.Black;

                Debug.WriteLine("╔════════════════════════════════════╗");
                Debug.WriteLine("║  ELD ENGINE - Inicializando...     ║");
                Debug.WriteLine("╚════════════════════════════════════╝");

                // 1. Crear servicios del motor
                _sceneService = new SceneService();
                _inputService = new WindowsFormsInputService(this);
                _gameService = new GameService(_sceneService, _inputService);

                Debug.WriteLine("✓ Servicios creados");

                // 2. Inicializar motor ECS
                _gameService.Initialize();
                Debug.WriteLine("✓ Motor ECS inicializado");


                // 3. Crear sistemas de colisión
                _collisionSystem = new CollisionSystem();
                _gameService.World.AddSystem(_collisionSystem);
                Debug.WriteLine("✓ Sistema de colisiones registrado");

                // 4. Crear sistema de IA
                _enemyAISystem = new EnemyAISystem(_collisionSystem);
                _gameService.World.AddSystem(_enemyAISystem);
                Debug.WriteLine("✓ Sistema de IA de enemigos registrado");

                // 5. Registrar sistemas mejorados de input y movimiento
                _gameService.World.AddSystem(new AdvancedPlayerInputSystem(_inputService, _collisionSystem));
                Debug.WriteLine("✓ Sistema de input mejorado registrado");

                _gameService.World.AddSystem(new SlidingMovementSystem(_collisionSystem));
                Debug.WriteLine("✓ Sistema de movimiento mejorado registrado");

                // 6. Registrar otros sistemas
                _gameService.World.AddSystem(new NpcInteractionSystem());
                _gameService.World.AddSystem(new CombatSystem());
                Debug.WriteLine("✓ Sistemas adicionales registrados");
                //_gameService.World.AddSystem(new GravitySystem());

                // 7. Crear GameManager
                _gameManager = new GameManager();
                _gameManager.Initialize(_gameService);

                // Suscribirse a eventos
                _gameManager.OnGameStateChanged += OnGameStateChanged;
                _gameManager.OnGameMessage += OnGameMessage;
                Debug.WriteLine("✓ GameManager inicializado");                

                // 8. Crear LevelManager
                _levelManager = new LevelManager();
                _levelManager.Initialize(_gameService, _sceneService, _gameManager);
                Debug.WriteLine("✓ LevelManager inicializado con 5 niveles");

                // 9. Inicializar rendering
                _renderContext = new GameRenderContext(this);
                Debug.WriteLine("✓ Contexto de renderizado inicializado");

                // 10. Crear UI
                CreateUIControls();
                Debug.WriteLine("✓ UI creada");

                // 11. Iniciar game loop
                _gameTimer = Stopwatch.StartNew();
                this.Paint += GameWindow_Paint;
                this.Resize += GameWindow_Resize;
                this.KeyDown += GameWindow_KeyDown;

                // Timer para actualizar UI (30 FPS)
                _uiUpdateTimer = new Timer();
                _uiUpdateTimer.Interval = 33;
                _uiUpdateTimer.Tick += UIUpdateTimer_Tick;
                _uiUpdateTimer.Start();

                // Forzar render continuo
                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                              ControlStyles.UserPaint |
                              ControlStyles.Opaque, true);

                // Invalidate cada frame
                var gameLoopTimer = new Timer();
                gameLoopTimer.Interval = 16; // ~60 FPS
                gameLoopTimer.Tick += (s, e) => this.Invalidate();
                gameLoopTimer.Start();

                Debug.WriteLine("✓ Game loop iniciado");

                // 12. Cambiar a menú
                _gameManager.SetGameState(GameState.Menu);

                Debug.WriteLine("╔════════════════════════════════════════╗");
                Debug.WriteLine("║  ✓ JUEGO LISTO                         ║");
                Debug.WriteLine("║  Presiona ESC o CTRL+Q en menú para    ║");
                Debug.WriteLine("║  salir del juego                       ║");
                Debug.WriteLine("╚════════════════════════════════════════╝\n");

                // Inicializar SpriteAssetManager
                var assetManager = new SpriteAssetManager();
                _gameService.SetAssetManager(assetManager);

                // Cargar spritesheets
                LoadSpritesheets(assetManager);

                // Agregar sistemas
                _gameService.World.AddSystem(
                    new AnimationSystem());
                _gameService.World.AddSystem(
                    new SpriteRenderSystem(assetManager, _renderContext));

                Console.WriteLine("[GameWindow] Sistemas de sprite inicializados");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error: {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Error inicializando juego: {ex.Message}\n\n{ex.StackTrace}",
                    "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadSpritesheets(SpriteAssetManager assetManager)
        {
            // Ejemplo: Cargar jugador
            assetManager.LoadSpriteSheet(
                spritesheetId: "player",
                imagePath: "player.png",
                tileWidth: 32,
                tileHeight: 32);

            // Ejemplo: Cargar enemigos
            assetManager.LoadSpriteSheet(
                spritesheetId: "enemies",
                imagePath: "enemies.png",
                tileWidth: 32,
                tileHeight: 32);

            Console.WriteLine("[LoadSpritesheets] Spritesheets cargados");
        }

        private void GameWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            // ==================== MENÚ PRINCIPAL ====================
            if (_gameManager.IsMenu)
            {
                // ESC para cerrar desde menú
                if (e.KeyCode == Keys.Escape)
                {
                    Debug.WriteLine("╔════════════════════════════════════╗");
                    Debug.WriteLine("║  CERRANDO JUEGO DESDE MENÚ         ║");
                    Debug.WriteLine("║  ¡Hasta luego, aventurero! 👋      ║");
                    Debug.WriteLine("╚════════════════════════════════════╝");

                    e.Handled = true;
                    this.Close();  // Cierra la aplicación
                    return;
                }

                // CTRL+Q para cerrar desde menú
                if (e.KeyCode == Keys.Q && e.Control)
                {
                    Debug.WriteLine("╔════════════════════════════════════╗");
                    Debug.WriteLine("║  CERRANDO JUEGO (CTRL+Q)          ║");
                    Debug.WriteLine("║  ¡Hasta luego, aventurero! 👋      ║");
                    Debug.WriteLine("╚════════════════════════════════════╝");

                    e.Handled = true;
                    this.Close();  // Cierra la aplicación
                    return;
                }

                // SPACE para comenzar
                if (e.KeyCode == Keys.Space)
                {
                    _levelManager.LoadLevel(0);
                    _gameManager.SetGameState(GameState.Playing);
                    e.Handled = true;
                    return;
                }

                return;  // IMPORTANTE: No procesar más teclas en menú
            }

            // ==================== DURANTE EL JUEGO ====================
            if (e.KeyCode == Keys.Escape)
            {
                if (_gameManager.IsPlaying)
                {
                    _gameManager.TogglePause();
                }
                else if (_gameManager.IsPaused)
                {
                    _gameManager.SetGameState(GameState.Playing);
                }
                e.Handled = true;
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.R:
                    if (!_gameManager.IsPlaying && !_gameManager.IsMenu)
                    {
                        _levelManager.RestartLevel();
                        _gameManager.Restart();
                        _gameManager.SetGameState(GameState.Playing);
                        e.Handled = true;
                    }
                    break;

                case Keys.M:
                    _gameManager.SetGameState(GameState.Menu);
                    e.Handled = true;
                    break;                

                case Keys.P:
                    // Agregar puntos (para testing)
                    _gameManager.AddScore(50);
                    e.Handled = true;
                    break;

                case Keys.L:
                    if (_gameManager.IsPlaying)
                    {
                        _levelManager.NextLevel();
                        //_gameManager.SetGameState(GameState.Playing);
                        e.Handled = true;
                    }
                    break;

                case Keys.G:
                    // Game over (para testing)
                    _gameManager.GameOver("Tecla G presionada");
                    e.Handled = true;
                    break;
            }
        }

        private void OnGameMessage(string message)
        {
            _messageLabel.Text = message;
            _messageLabel.Visible = true;

            // Ocultar después de 3 segundos
            var timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) =>
            {
                _messageLabel.Visible = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void OnGameStateChanged(GameState newState)
        {
            System.Diagnostics.Debug.WriteLine($"Estado cambió a: {newState}");
        }

        private void UIUpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (_gameManager == null || _levelManager == null) return;

            // Actualizar UI
            var fps = (int)(1f / (_deltaTime > 0 ? _deltaTime : 0.016f));
            var entityCount = _gameService.World.GetEntitiesWith<Transform>().Count();

            _statusLabel.Text = $"FPS: {fps} | Entities: {entityCount} | Time: {_gameManager.PlayTime:F1}s";
            _scoreLabel.Text = $"Score: {_gameManager.CurrentScore}";
            _levelLabel.Text = $"Level: {_levelManager.CurrentLevel}/{_levelManager.TotalLevels}";

            // Estado visual
            var stateIcon = _gameManager.CurrentState switch
            {
                GameState.Playing => "▶️",
                GameState.Paused => "⏸️",
                GameState.GameOver => "💀",
                GameState.Menu => "🎮",
                _ => "❓"
            };

            _stateLabel.Text = $"{stateIcon} {_gameManager.CurrentState}";
            _stateLabel.ForeColor = _gameManager.CurrentState switch
            {
                GameState.Playing => System.Drawing.Color.Lime,
                GameState.Paused => System.Drawing.Color.Yellow,
                GameState.GameOver => System.Drawing.Color.Red,
                GameState.Menu => System.Drawing.Color.Cyan,
                _ => System.Drawing.Color.White
            };
        }

        private void GameWindow_Resize(object? sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void GameWindow_Paint(object? sender, PaintEventArgs e)
        {
            try
            {
                _deltaTime = (float)_gameTimer.Elapsed.TotalSeconds;
                _gameTimer.Restart();

                if (_deltaTime > 0.05f) _deltaTime = 0.05f; // Cap deltaTime

                // Actualizar GameManager
                _gameManager.Update(_deltaTime);

                // Solo actualizar lógica si no está pausado
                if (_gameManager.IsPlaying)
                {
                    _gameService.Update(_deltaTime);
                }

                // Renderizar siempre
                _renderContext.Render(e.Graphics, _gameService.World, this.ClientSize);

                // Renderizar overlay según estado
                RenderStateOverlay(e.Graphics);
            }
            catch (Exception ex)
            {
                e.Graphics.DrawString($"Error: {ex.Message}",
                    new System.Drawing.Font("Arial", 10),
                    System.Drawing.Brushes.Red, 10, 10);
            }
        }

        private void RenderStateOverlay(Graphics g)
        {
            switch (_gameManager.CurrentState)
            {
                case GameState.Paused:
                    RenderPauseOverlay(g);
                    break;

                case GameState.GameOver:
                    RenderGameOverOverlay(g);
                    break;

                case GameState.Menu:
                    RenderMenuOverlay(g);
                    break;
            }
        }

        private void RenderMenuOverlay(Graphics g)
        {
            g.FillRectangle(
                new System.Drawing.SolidBrush(System.Drawing.Color.Black),
                0, 0, this.ClientSize.Width, this.ClientSize.Height);

            var centerX = this.ClientSize.Width / 2;
            var centerY = this.ClientSize.Height / 2;

            // Title
            var titleFont = new System.Drawing.Font("Arial", 48f, System.Drawing.FontStyle.Bold);
            var titleText = "ELDER";
            var titleSize = g.MeasureString(titleText, titleFont);
            g.DrawString(titleText, titleFont,
                System.Drawing.Brushes.Cyan,
                centerX - titleSize.Width / 2,
                centerY - 150);

            // Subtitle
            var subtitleFont = new System.Drawing.Font("Arial", 18f);
            var subtitleText = "Chronicles of the Silver Grove";
            var subtitleSize = g.MeasureString(subtitleText, subtitleFont);
            g.DrawString(subtitleText, subtitleFont, 
                System.Drawing.Brushes.LimeGreen,
                centerX - subtitleSize.Width / 2,
                centerY - 80);

            // Menu options
            var menuFont = new System.Drawing.Font("Arial", 16f);
            var menuTesxt = "Press SPACE to Start";
            var menuSize = g.MeasureString(menuTesxt, menuFont);
            g.DrawString(menuTesxt,
                menuFont,
                System.Drawing.Brushes.White,
                centerX - menuSize.Width / 2,
                centerY + 50);

            // Mostrar opción de salir
            var smallFont = new System.Drawing.Font("Arial", 12f);
            var smallText = "WASD Movement | E/Q/R Actions | ESC or CTRL+Q to Exit";
            var smallSize = g.MeasureString(smallText, smallFont);
            g.DrawString(smallText,
                smallFont, System.Drawing.Brushes.LightGray,
                centerX - smallSize.Width / 2, 
                centerY + 130);
            // Descripción del juego
            var descFont = new System.Drawing.Font("Arial", 12f, FontStyle.Italic);
            var descText = "5 Levels | Enemies AI | Collisions";
            var descSize = g.MeasureString(descText, descFont);
            g.DrawString(descText, descFont,
                System.Drawing.Brushes.LightGray,
                centerX - descSize.Width / 2,
                centerY + 160);
        }

        private void RenderGameOverOverlay(Graphics g)
        {
            g.FillRectangle(
                new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(150, 0, 0, 0)),
                0, 0, this.ClientSize.Width, this.ClientSize.Height);

            var centerX = this.ClientSize.Width / 2;
            var centerY = this.ClientSize.Height / 2;

            // Game Over text
            var gameOverFont = new System.Drawing.Font("Arial", 52f, System.Drawing.FontStyle.Bold);
            var gameOverText = "💀 GAME OVER 💀";
            var gameOverSize = g.MeasureString(gameOverText, gameOverFont);
            g.DrawString(gameOverText, gameOverFont,
                System.Drawing.Brushes.Red,
                centerX - gameOverSize.Width / 2,
                centerY - 100);

            // Score
            var scoreFont = new System.Drawing.Font("Arial", 20f);
            var scoreText = $"Final Score: {_gameManager.CurrentScore}";
            var scoreSize = g.MeasureString(scoreText, scoreFont);
            g.DrawString(scoreText,
                scoreFont,
                System.Drawing.Brushes.Yellow,
                centerX - scoreSize.Width / 2,
                centerY + 20);

            // Level reached
            var levelFont = new System.Drawing.Font("Arial", 20f);
            var levelText = $"Level Reached: {_levelManager.CurrentLevel}/{_levelManager.TotalLevels}";
            var levelSize = g.MeasureString(levelText, levelFont);
            g.DrawString(levelText,
                levelFont,
                System.Drawing.Brushes.Yellow,
                centerX - levelSize.Width / 2,
                centerY + 60);

            // Instructions
            var infoFont = new System.Drawing.Font("Arial", 14f);
            var infoText = "Press R to Restart | M for Menu";
            var infoSize = g.MeasureString(infoText, infoFont);
            g.DrawString(infoText,
                infoFont,
                System.Drawing.Brushes.White,
                centerX - infoSize.Width / 2,
                centerY + 130);
        }

        private void RenderPauseOverlay(Graphics g)
        {
            // Oscurecer pantalla
            g.FillRectangle(
                new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(100, 0, 0, 0)),
                0, 0, this.ClientSize.Width, this.ClientSize.Height);

            var centerX = this.ClientSize.Width / 2;
            var centerY = this.ClientSize.Height / 2;

            // Texto "PAUSED"
            var pauseFont = new System.Drawing.Font("Arial", 48f, System.Drawing.FontStyle.Bold);
            var pauseText = "⏸️ PAUSED";
            var pauseSize = g.MeasureString(pauseText, pauseFont);
            g.DrawString(pauseText, pauseFont,
                System.Drawing.Brushes.Yellow,
                centerX - pauseSize.Width / 2,
                centerY - 100);

            // Instrucciones
            var infoFont = new System.Drawing.Font("Arial", 14f);
            var infoText = "Press ESC to Resume | R to Restart | M for Menu";
            var infoSize = g.MeasureString(infoText, infoFont);
            g.DrawString(infoText,
                infoFont,
                System.Drawing.Brushes.White,
                centerX - infoSize.Width / 2,
                centerY + 50);
        }

        private void CreateUIControls()
        {
            // Estado del juego
            _stateLabel = new Label
            {
                Text = "🎮 MENU",
                ForeColor = System.Drawing.Color.Cyan,
                BackColor = System.Drawing.Color.Transparent,
                AutoSize = true,
                Location = new System.Drawing.Point(10, 10),
                Font = new System.Drawing.Font("Consolas", 12f, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(_stateLabel);
            _stateLabel.BringToFront();

            // Nivel actual
            _levelLabel = new Label
            {
                Text = "Level: 0/5",
                ForeColor = System.Drawing.Color.Yellow,
                BackColor = System.Drawing.Color.Transparent,
                AutoSize = true,
                Location = new System.Drawing.Point(10, 40),
                Font = new System.Drawing.Font("Consolas", 11f, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(_levelLabel);
            _levelLabel.BringToFront();

            // Score
            _scoreLabel = new Label
            {
                Text = "Score: 0",
                ForeColor = System.Drawing.Color.Lime,
                BackColor = System.Drawing.Color.Transparent,
                AutoSize = true,
                Location = new System.Drawing.Point(10, 70),
                Font = new System.Drawing.Font("Consolas", 11f, System.Drawing.FontStyle.Bold)
            };
            this.Controls.Add(_scoreLabel);
            _scoreLabel.BringToFront();

            // Status
            _statusLabel = new Label
            {
                Text = "FPS: 0 | Entities: 0",
                ForeColor = System.Drawing.Color.Cyan,
                BackColor = System.Drawing.Color.Transparent,
                AutoSize = true,
                Location = new System.Drawing.Point(10, 100),
                Font = new System.Drawing.Font("Consolas", 9f)
            };
            this.Controls.Add(_statusLabel);
            _statusLabel.BringToFront();

            // Mensaje temporal
            _messageLabel = new Label
            {
                Text = "",
                ForeColor = System.Drawing.Color.LimeGreen,
                BackColor = System.Drawing.Color.Transparent,
                AutoSize = false,
                Location = new System.Drawing.Point(this.Width / 2 - 200, 100),
                Width = 400,
                Height = 60,
                Font = new System.Drawing.Font("Arial", 14f, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.TopCenter
            };
            this.Controls.Add(_messageLabel);
            _messageLabel.BringToFront();
        }        
    }
}
