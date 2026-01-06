using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    public struct NpcBehavior : IComponent
    {
        public string NpcName { get; set; }
        public string DialogueKey { get; set; }
        public bool CanTrade { get; set; }
        public int XpReward { get; set; }
        public float InteractionDistance { get; set; }

        public NpcBehavior(string name, string dialogue)
        {
            NpcName = name;
            DialogueKey = dialogue;
            CanTrade = false;
            XpReward = 0;
            InteractionDistance = 2f;
        }

        static int IComponent.GetComponentTypeId() => typeof(NpcBehavior).GetHashCode();
    }
}
