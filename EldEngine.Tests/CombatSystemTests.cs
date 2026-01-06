using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using EldEngine.GameTest.Systems;

namespace EldEngine.Tests
{
    public class CombatSystemTests
    {
        [Fact]
        public void ResolveAttack_ReduceEnemyHealth()
        {
            // Arrange
            var world = new World();
            var attacker = world.CreateEntity();
            var defender = world.CreateEntity();

            world.AddComponent(attacker, new CombatStats(100, 20, 0));
            world.AddComponent(defender, new CombatStats(100, 10, 0));

            var combatSystem = new CombatSystem();

            // Act
            var damage = combatSystem.ResolveAttack(world, attacker, defender);
            var defenderStats = world.GetComponent<CombatStats>(defender);

            // Assert
            Assert.True(damage > 0);
            Assert.True(defenderStats.CurrentHealth < 100);
        }

        [Fact]
        public void CombatStats_TakeDamage_CannotGoBelow0()
        {
            var stats = new CombatStats(50, 10, 0);
            stats.TakeDamage(100);

            Assert.Equal(0, stats.CurrentHealth);
        }
    }
}
