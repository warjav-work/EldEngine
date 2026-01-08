using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    public struct InventoryItem
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public string ItemName { get; set; }
    }

    public class InventoryComponent : IComponent
    {
        public List<InventoryItem> Items { get; private set; } = new();
        public int MaxSlots { get; set; } = 20;

        public void AddItem(InventoryItem item)
        {
            if (Items.Count < MaxSlots)
                Items.Add(item);
        }

        public bool RemoveItem(string itemId)
        {
            var item = Items.Find(x => x.ItemId == itemId);
            return Items.Remove(item);
        }

        public InventoryItem GetItem(string itemId) =>
            Items.Find(x => x.ItemId == itemId);

        static int IComponent.GetComponentTypeId() => typeof(InventoryComponent).GetHashCode();
    }
}
