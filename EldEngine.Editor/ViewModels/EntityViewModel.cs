using EldEngine.Core.Domain.Entities;
using System.Collections.ObjectModel;

namespace EldEngine.Editor.ViewModels
{
    public class EntityViewModel
    {
        public Entity Entity { get; set; }
        public string Name { get; set; }
        public ObservableCollection<ComponentViewModel> Components { get; set; } = new();
        public bool IsSelected { get; set; }
    }
}
