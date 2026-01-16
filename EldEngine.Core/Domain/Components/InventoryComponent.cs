using EldEngine.Core.Domain.Entities;

namespace EldEngine.Core.Domain.Components
{
    public class InventoryComponent : ComponentBase
    {
        public List<string> Items { get; private set; } = new();
        public int MaxSlots { get; set; } = 20;

        public bool AddItem(string itemId)
        {
            if (Items.Count >= MaxSlots)
                return false;

            Items.Add(itemId);
            return true;
        }

        public bool RemoveItem(string itemId)
        {
            return Items.Remove(itemId);
        }

        public bool HasItem(string itemId)
        {
            return Items.Contains(itemId);
        }

        /// <summary>
        /// Validación: cantidad de items no puede exceder máximo.
        /// </summary>
        public override bool IsValid()
        {
            return Items.Count <= MaxSlots && Items.Count >= 0;
        }

        /// <summary>
        /// Reset: vaciar inventario.
        /// </summary>
        public override void Reset()
        {
            Items.Clear();
        }

        /// <summary>
        /// Clonación: crear copia profunda de items.
        /// </summary>
        public override ComponentBase Clone()
        {
            var cloned = new InventoryComponent { MaxSlots = this.MaxSlots };
            cloned.Items.AddRange(this.Items);
            return cloned;
        }

        public override string ToString()
        {
            return $"Inventory({Items.Count}/{MaxSlots}) [Items: {string.Join(", ", Items)}]";
        }
    }
}
