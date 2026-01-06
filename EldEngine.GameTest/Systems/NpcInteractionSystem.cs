using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Detecta proximidad con NPCs y permite interacción.
    /// </summary>
    public class NpcInteractionSystem : ISystem
    {
        public string Name => nameof(NpcInteractionSystem);
        public int Priority => 80;

        public event System.Action<Entity> OnNpcInteraction;

        public void Execute(World world, float deltaTime)
        {
            var players = world.GetEntitiesWith<PlayerController, Transform>().ToList();
            var npcs = world.GetEntitiesWith<NpcBehavior, Transform>().ToList();

            foreach (var player in players)
            {
                var playerPos = world.GetComponent<Transform>(player);

                foreach (var npc in npcs)
                {
                    var npcPos = world.GetComponent<Transform>(npc);
                    var npcBehavior = world.GetComponent<NpcBehavior>(npc);

                    // Calcular distancia
                    var dx = playerPos.X - npcPos.X;
                    var dy = playerPos.Y - npcPos.Y;
                    var distance = System.Math.Sqrt(dx * dx + dy * dy);

                    if (distance < npcBehavior.InteractionDistance)
                    {
                        OnNpcInteraction?.Invoke(npc);
                    }
                }
            }
        }
    }
}
