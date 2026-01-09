using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using System.Diagnostics;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Sistema que aplica daño por contacto/proximidad.
    /// Se ejecuta DESPUÉS del movimiento para no bloquear el desplazamiento.
    /// </summary>
    public class ContactDamageSystem : ISystem
    {
        private Dictionary<(int, int), float> _lastDamageTime = new();
        private const float DAMAGE_COOLDOWN = 1.0f; // Segundos entre daños

        public string Name => nameof(ContactDamageSystem);
        public int Priority => 35; // DESPUÉS de movimiento (22) pero ANTES de combat (100)

        public void Execute(World world, float deltaTime)
        {
            var players = world.GetEntitiesWith<Transform, ColliderComponent, CombatStats>()
                .Where(e => world.HasComponent<PlayerController>(e))
                .ToList();

            var enemies = world.GetEntitiesWith<Transform, ColliderComponent, CombatStats, EnemyAIComponent>()
                .ToList();

            // Verificar contacto jugador-enemigos
            foreach (var player in players)
            {
                foreach (var enemy in enemies)
                {
                    CheckDamageContact(world, player, enemy, deltaTime);
                }
            }
        }

        /// <summary>
        /// Verifica si hay contacto entre dos entidades y aplica daño si corresponde.
        /// </summary>
        private void CheckDamageContact(World world, Entity entity1, Entity entity2, float deltaTime)
        {
            var t1 = world.GetComponent<Transform>(entity1);
            var c1 = world.GetComponent<ColliderComponent>(entity1);
            var t2 = world.GetComponent<Transform>(entity2);
            var c2 = world.GetComponent<ColliderComponent>(entity2);

            // Verificar colisión AABB simple
            if (!IsColliding(t1, c1, t2, c2))
                return;

            // Obtener clave de contacto
            var key = GetContactKey(entity1.Id, entity2.Id);

            // Verificar cooldown
            if (_lastDamageTime.TryGetValue(key, out float lastTime))
            {
                if (deltaTime - lastTime < DAMAGE_COOLDOWN)
                    return; // Aún en cooldown
            }

            // Aplicar daño
            var stats1 = world.GetComponent<CombatStats>(entity1);
            var stats2 = world.GetComponent<CombatStats>(entity2);

            // Enemigo daña al jugador
            if (world.HasComponent<PlayerController>(entity1) &&
                world.HasComponent<EnemyAIComponent>(entity2))
            {
                stats1.CurrentHealth -= stats2.Attack;
                _lastDamageTime[key] = deltaTime;

                Debug.WriteLine($"[DAMAGE] ¡Jugador recibió {stats2.Attack} de daño! " +
                    $"Vida: {stats1.CurrentHealth}/{stats1.MaxHealth}");

                world.RemoveComponent<CombatStats>(entity1);
                world.AddComponent(entity1, stats1);
            }
            // Jugador daña a enemigo
            else if (world.HasComponent<PlayerController>(entity2) &&
                     world.HasComponent<EnemyAIComponent>(entity1))
            {
                stats1.CurrentHealth -= stats2.Attack;
                _lastDamageTime[key] = deltaTime;

                Debug.WriteLine($"[DAMAGE] ¡Enemigo recibió {stats2.Attack} de daño! " +
                    $"Vida: {stats1.CurrentHealth}/{stats1.MaxHealth}");

                world.RemoveComponent<CombatStats>(entity1);
                world.AddComponent(entity1, stats1);
            }
        }

        /// <summary>
        /// Verifica colisión AABB simple.
        /// </summary>
        private bool IsColliding(Transform t1, ColliderComponent c1, Transform t2, ColliderComponent c2)
        {
            float left1 = t1.X - c1.Width / 2f;
            float right1 = t1.X + c1.Width / 2f;
            float top1 = t1.Y - c1.Height / 2f;
            float bottom1 = t1.Y + c1.Height / 2f;

            float left2 = t2.X - c2.Width / 2f;
            float right2 = t2.X + c2.Width / 2f;
            float top2 = t2.Y - c2.Height / 2f;
            float bottom2 = t2.Y + c2.Height / 2f;

            return !(right1 < left2 || left1 > right2 || bottom1 < top2 || top1 > bottom2);
        }

        private (int, int) GetContactKey(int id1, int id2)
            => id1 < id2 ? (id1, id2) : (id2, id1);
    }
}
