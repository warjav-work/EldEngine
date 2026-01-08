using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Systems
{
    public class InventorySystem : ISystem
    {
        public string Name => nameof(InventorySystem);
        public int Priority => 75;

        // Eventos para comunicación entre sistemas
        public event System.Action<Entity, InventoryItem> OnItemPickup;
        public event System.Action<Entity, string> OnItemUse;

        public void Execute(World world, float deltaTime)
        {
            var inventories = world.GetEntitiesWith<InventoryComponent, Transform>();
            // Lógica...
        }

        // Método público para que otros sistemas lo usen
        public void AddItemToInventory(World world, Entity entity, InventoryItem item)
        {
            if (world.HasComponent<InventoryComponent>(entity))
            {
                var inv = world.GetComponent<InventoryComponent>(entity);
                inv.AddItem(item);
                world.RemoveComponent<InventoryComponent>(entity);
                world.AddComponent(entity, inv);

                OnItemPickup?.Invoke(entity, item);
            }
        }
    }
}
