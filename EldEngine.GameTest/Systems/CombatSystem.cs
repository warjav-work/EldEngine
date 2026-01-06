using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Gestiona combates, cálculo de daño, muertes.
    /// </summary>
    public class CombatSystem : ISystem
    {
        public string Name => nameof(CombatSystem);
        public int Priority => 100;

        public event System.Action<Entity> OnEntityDefeated;

        public void Execute(World world, float deltaTime)
        {
            var combatantes = world.GetEntitiesWith<CombatStats>().ToList();

            // Ejemplo simplificado: verificar muertes
            foreach (var entity in combatantes)
            {
                var stats = world.GetComponent<CombatStats>(entity);

                if (!stats.IsAlive)
                {
                    OnEntityDefeated?.Invoke(entity);
                }
            }
        }

        /// <summary>Resuelve un ataque entre dos entidades</summary>
        public int ResolveAttack(World world, Entity attacker, Entity defender)
        {
            var attackerStats = world.GetComponent<CombatStats>(attacker);
            var defenderStats = world.GetComponent<CombatStats>(defender);

            var damage = attackerStats.DealDamage();
            var actualDamage = defenderStats.TakeDamage(damage);

            // Actualizar componente del defensor
            world.RemoveComponent<CombatStats>(defender);
            world.AddComponent(defender, defenderStats);

            return actualDamage;
        }
    }
}
