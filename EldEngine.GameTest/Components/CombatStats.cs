using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    /// <summary>
    /// Estadísticas de combate compartidas por jugador y enemigos.
    /// </summary>
    public struct CombatStats : IComponent
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public float CritChance { get; set; }
        public bool IsAlive => CurrentHealth > 0;

        public CombatStats(int maxHp, int attack, int defense)
        {
            MaxHealth = maxHp;
            CurrentHealth = maxHp;
            Attack = attack;
            Defense = defense;
            CritChance = 0.15f;
        }

        public int TakeDamage(int incomingDamage)
        {
            var actualDamage = Math.Max(1, incomingDamage - Defense / 2);
            CurrentHealth = Math.Max(0, CurrentHealth - actualDamage);
            return actualDamage;
        }

        public int DealDamage()
        {
            var isCrit = new Random().NextDouble() < CritChance;
            return isCrit ? Attack * 2 : Attack;
        }

        static int IComponent.GetComponentTypeId() => typeof(CombatStats).GetHashCode();
    }
}
