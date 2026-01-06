using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Application.Services;
using EldEngine.Editor.Services;
using EldEngine.Editor.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using InputManager = EldEngine.Core.Application.Services.InputManager;


namespace EldEngine.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EditorService _editorService;
        private IGameService _gameService;
        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
        }
        private void InitializeServices()
        {
            try
            {
                // Configuración de inyección de dependencias
                var sceneService = new SceneService();
                var inputService = new InputManager();

                _gameService = new GameService(sceneService, inputService);
                _gameService.Initialize();

                _editorService = new EditorService(_gameService, sceneService);

                // Bind UI
                EntityListView.ItemsSource = _editorService.Entities;

                // Suscribirse a eventos
                _editorService.OnEntityCreated += OnEntityCreated;
                _editorService.OnEntitySelected += OnEntitySelected;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inicializando editor: {ex.Message}",
                    "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusBar.Text = "✗ Error de inicialización";
            }
        }

        // ==================== EVENT HANDLERS ====================

        private void OnCreateEntityClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var entity = _editorService.CreateEntity($"Entity_{_editorService.Entities.Count + 1}");
                EntityListView.SelectedItem = entity;
                StatusBar.Text = $"✓ Entidad creada: {entity.Name}";
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"✗ Error creando entidad: {ex.Message}";
            }
        }

        private void OnDeleteEntityClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EntityListView.SelectedItem is EntityViewModel entity)
                {
                    _editorService.DeleteEntity(entity);
                    PropertiesPanel.Children.Clear();
                    StatusBar.Text = $"✓ Entidad eliminada: {entity.Name}";
                }
                else
                {
                    StatusBar.Text = "⚠ Selecciona una entidad primero";
                }
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"✗ Error eliminando entidad: {ex.Message}";
            }
        }

        private void OnEntityCreated(EntityViewModel entity)
        {
            StatusBar.Text = $"Entidad creada: {entity.Name}";
        }

        private void OnEntitySelected(EntityViewModel entity)
        {
            if (entity != null)
            {
                DisplayEntityProperties(entity);
                DebugInfo.Text = _editorService.GetEntityDebugInfo(entity);
            }
        }

        private void DisplayEntityProperties(EntityViewModel entity)
        {
            PropertiesPanel.Children.Clear();

            // Título
            var title = new TextBlock
            {
                Text = $"Propiedades: {entity.Name}",
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(10, 5, 10, 10)
            };
            PropertiesPanel.Children.Add(title);

            // Información básica
            var idInfo = new TextBlock
            {
                Text = $"ID: {entity.Entity.Id}",
                Foreground = System.Windows.Media.Brushes.LightGray,
                Padding = new Thickness(10, 0, 10, 5)
            };
            PropertiesPanel.Children.Add(idInfo);

            // Componentes
            var compTitle = new TextBlock
            {
                Text = "Componentes:",
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                Padding = new Thickness(10, 10, 10, 5),
                Margin = new Thickness(0, 10, 0, 0)
            };
            PropertiesPanel.Children.Add(compTitle);

            if (entity.Components.Count == 0)
            {
                var noComp = new TextBlock
                {
                    Text = "Sin componentes",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Padding = new Thickness(20, 5, 10, 5),
                    FontStyle = FontStyles.Italic
                };
                PropertiesPanel.Children.Add(noComp);
            }
            else
            {
                foreach (var comp in entity.Components)
                {
                    var compBlock = new TextBlock
                    {
                        Text = $"• {comp.Type}",
                        Foreground = System.Windows.Media.Brushes.LightCyan,
                        Padding = new Thickness(20, 5, 10, 5),
                        TextWrapping = TextWrapping.Wrap
                    };
                    PropertiesPanel.Children.Add(compBlock);
                }
            }
        }

        private void OnSaveSceneClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "Scene Files (*.scene)|*.scene|All Files (*.*)|*.*",
                    DefaultExt = "scene"
                };

                if (dialog.ShowDialog() == true)
                {
                    _editorService.SaveScene(dialog.FileName);
                    StatusBar.Text = $"✓ Escena guardada: {dialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"✗ Error guardando escena: {ex.Message}";
            }
        }

        private void OnLoadSceneClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Scene Files (*.scene)|*.scene|All Files (*.*)|*.*"
                };

                if (dialog.ShowDialog() == true)
                {
                    _editorService.LoadScene(dialog.FileName);
                    StatusBar.Text = $"✓ Escena cargada: {dialog.FileName}";
                }
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"✗ Error cargando escena: {ex.Message}";
            }
        }
        private void OnEntityListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (EntityListView?.SelectedItem is EntityViewModel entity)
                {
                    _editorService.SelectEntity(entity);
                }
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"✗ Error: {ex.Message}";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _gameService?.Dispose();
        }
    }
}