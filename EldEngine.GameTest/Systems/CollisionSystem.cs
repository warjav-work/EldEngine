using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Detecta colisiones entre entidades.
    /// </summary>
    public class CollisionSystem : ISystem
    {
        public string Name => nameof(CollisionSystem);
        public int Priority => 30;  // Después de movimiento, antes de interacción

        public event Action<Entity, Entity> OnCollisionDetected;
        public event Action<Entity, Entity> OnCollisionSeparated;

        private Dictionary<(int, int), bool> _previousCollisions = new();

        public void Execute(World world, float deltaTime)
        {
            var collidables = world.GetEntitiesWith<Transform, ColliderComponent>().ToList();

            // Detectar nuevas colisiones
            for (int i = 0; i < collidables.Count; i++)
            {
                for (int j = i + 1; j < collidables.Count; j++)
                {
                    var entity1 = collidables[i];
                    var entity2 = collidables[j];

                    if (!entity1.IsValid || !entity2.IsValid) continue;

                    var transform1 = world.GetComponent<Transform>(entity1);
                    var collider1 = world.GetComponent<ColliderComponent>(entity1);

                    var transform2 = world.GetComponent<Transform>(entity2);
                    var collider2 = world.GetComponent<ColliderComponent>(entity2);

                    bool isColliding = CheckCollision(transform1, collider1, transform2, collider2);
                    var key = (entity1.Id, entity2.Id);

                    bool wasColliding = _previousCollisions.ContainsKey(key) && _previousCollisions[key];

                    if (isColliding && !wasColliding)
                    {
                        // Colisión nueva
                        OnCollisionDetected?.Invoke(entity1, entity2);
                        _previousCollisions[key] = true;

                        System.Diagnostics.Debug.WriteLine(
                            $"[COLLISION] {collider1.Tag} ({entity1.Id}) ↔ {collider2.Tag} ({entity2.Id})");
                    }
                    else if (!isColliding && wasColliding)
                    {
                        // Colisión finalizada
                        OnCollisionSeparated?.Invoke(entity1, entity2);
                        _previousCollisions[key] = false;
                    }
                    else if (isColliding)
                    {
                        _previousCollisions[key] = true;
                    }
                }
            }
        }

        private bool CheckCollision(Transform t1, ColliderComponent c1, Transform t2, ColliderComponent c2)
        {
            // AABB (Axis-Aligned Bounding Box) collision
            float left1 = t1.X - c1.Width / 2;
            float right1 = t1.X + c1.Width / 2;
            float top1 = t1.Y - c1.Height / 2;
            float bottom1 = t1.Y + c1.Height / 2;

            float left2 = t2.X - c2.Width / 2;
            float right2 = t2.X + c2.Width / 2;
            float top2 = t2.Y - c2.Height / 2;
            float bottom2 = t2.Y + c2.Height / 2;

            return !(right1 < left2 || left1 > right2 || bottom1 < top2 || top1 > bottom2);
        }

        /// <summary>
        /// Resuelve colisiones sólidas (movimiento bloqueado).
        /// </summary>
        public bool CanMoveTo(World world, Entity entity, float newX, float newY)
        {
            if (!world.HasComponent<ColliderComponent>(entity))
                return true;

            var entityCollider = world.GetComponent<ColliderComponent>(entity);
            var testTransform = new Transform(newX, newY);

            var obstacles = world.GetEntitiesWith<Transform, ColliderComponent>().ToList();

            foreach (var obstacle in obstacles)
            {
                if (obstacle.Id == entity.Id) continue;
                if (!obstacle.IsValid) continue;

                var obsCollider = world.GetComponent<ColliderComponent>(obstacle);
                if (!obsCollider.IsSolid) continue;

                var obsTransform = world.GetComponent<Transform>(obstacle);

                if (CheckCollision(testTransform, entityCollider, obsTransform, obsCollider))
                {
                    return false; // Movimiento bloqueado
                }
            }

            return true; // Movimiento permitido
        }
    }
}
