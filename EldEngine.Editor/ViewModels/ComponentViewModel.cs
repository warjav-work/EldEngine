using System.Collections.ObjectModel;

namespace EldEngine.Editor.ViewModels
{
    public class ComponentViewModel
    {
        public string Type { get; set; }
        public object Component { get; set; }
        public ObservableCollection<PropertyViewModel> Properties { get; set; } = new();
    }
}
