using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Entities;
using EldEngine.Editor.ViewModels;
using System.Collections.ObjectModel;
using System.IO;

namespace EldEngine.Editor.Services
{
    /// <summary>
    /// Servicio de editor completamente desacoplado del runtime.
    /// </summary>
    public class EditorService
    {
        private readonly IGameService _gameService;
        private readonly ISceneService _sceneService;
        private Entity _selectedEntity = default;
        private ObservableCollection<EntityViewModel> _entities;

        public EntityViewModel SelectedEntity { get; set; }
        public ObservableCollection<EntityViewModel> Entities => _entities ??= new();

        public event Action<EntityViewModel> OnEntitySelected;
        public event Action<EntityViewModel> OnEntityCreated;
        public event Action<EntityViewModel> OnEntityDeleted;

        public EditorService(IGameService gameService, ISceneService sceneService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
        }

        // ==================== ENTITY OPERATIONS ====================

        public EntityViewModel CreateEntity(string name = "New Entity")
        {
            var entity = _gameService.World.CreateEntity();
            var viewModel = new EntityViewModel
            {
                Entity = entity,
                Name = name
            };

            Entities.Add(viewModel);
            OnEntityCreated?.Invoke(viewModel);

            return viewModel;
        }

        public void DeleteEntity(EntityViewModel viewModel)
        {
            _gameService.World.DestroyEntity(viewModel.Entity);
            Entities.Remove(viewModel);
            OnEntityDeleted?.Invoke(viewModel);
        }

        public void SelectEntity(EntityViewModel viewModel)
        {
            SelectedEntity = viewModel;
            OnEntitySelected?.Invoke(viewModel);
        }

        // ==================== COMPONENT OPERATIONS ====================

        public void AddComponent(EntityViewModel entity, Type componentType, object component)
        {
            var method = _gameService.World.GetType()
                .GetMethod("AddComponent")
                .MakeGenericMethod(componentType);

            method.Invoke(_gameService.World, new[] { entity.Entity, component });

            entity.Components.Add(new ComponentViewModel
            {
                Type = componentType.Name,
                Component = component
            });
        }

        public void RemoveComponent(EntityViewModel entity, string componentName)
        {
            var component = entity.Components.FirstOrDefault(c => c.Type == componentName);
            if (component == null) return;

            var componentType = Type.GetType(componentName);
            var method = _gameService.World.GetType()
                .GetMethod("RemoveComponent")
                .MakeGenericMethod(componentType);

            method.Invoke(_gameService.World, new object[] { entity.Entity });
            entity.Components.Remove(component);
        }

        // ==================== SCENE OPERATIONS ====================

        public void SaveScene(string path) =>
            _gameService.SaveScene(path);

        public void LoadScene(string path) =>
            _gameService.LoadScene(Path.GetFileNameWithoutExtension(path));

        // ==================== DEBUG ====================

        public string GetEntityDebugInfo(EntityViewModel entity)
        {
            var info = $"Entity: {entity.Name}\n";
            info += $"ID: {entity.Entity.Id}\n";
            info += $"Components: {entity.Components.Count}\n";
            foreach (var comp in entity.Components)
                info += $"  - {comp.Type}\n";
            return info;
        }
    }
}
