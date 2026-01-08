using EldEngine.Core.Domain.Entities;
using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using System.Diagnostics;
using System.Windows.Controls;

namespace EldEngine.GameTest.Systems
{
    /// <summary>
    /// Detecta colisiones entre entidades.
    /// </summary>
    public class CollisionSystem : ISystem
    {
        public string Name => nameof(CollisionSystem);
        public int Priority => 28;  // Antes de movimiento

        public event Action<Entity, Entity> OnCollisionDetected;
        public event Action<Entity, Entity> OnCollisionSeparated;
        private int _gridSize = 100;
        private bool _debugMode = false; // Para debugging

        private Dictionary<(int, int), bool> _previousCollisions = new();

        public void Execute(World world, float deltaTime)
        {
            
            var grid = CreateSpatialGrid(world);

            // Detectar nuevas colisiones
            foreach (var cell in grid.Values)
            {
                for (int i = 0; i < cell.Count; i++)
                {
                    for (int j = i + 1; j < cell.Count; j++)
                    {
                        CheckCollision(world, cell[i], cell[j]);
                    }
                }
            }
        }

        private Dictionary<(int, int), List<Entity>> CreateSpatialGrid(World world)
        {
            var grid = new Dictionary<(int, int), List<Entity>>();

            foreach (var entity in world.GetEntitiesWith<Transform, ColliderComponent>().ToList())
            {
                var transform = world.GetComponent<Transform>(entity);
                int gridX = (int)(transform.X / _gridSize);
                int gridY = (int)(transform.Y / _gridSize);
                var key = (gridX, gridY);

                if (!grid.ContainsKey(key))
                    grid[key] = new List<Entity>();

                grid[key].Add(entity);
            }

            return grid;
        }

        private void CheckCollision(World world, Entity entity1, Entity entity2)
        {
            if (!entity1.IsValid || !entity2.IsValid) return;

            var transform1 = world.GetComponent<Transform>(entity1);
            var collider1 = world.GetComponent<ColliderComponent>(entity1);
            var transform2 = world.GetComponent<Transform>(entity2);
            var collider2 = world.GetComponent<ColliderComponent>(entity2);

            bool isColliding = CheckAABBCollision(transform1, collider1, transform2, collider2);
            var key = GetCollisionKey(entity1.Id, entity2.Id);

            bool wasColliding = _previousCollisions.GetValueOrDefault(key, false);

            if (isColliding && !wasColliding)
            {
                OnCollisionDetected?.Invoke(entity1, entity2);
                _previousCollisions[key] = true;

                if (_debugMode)
                    Debug.WriteLine($"[COLLISION] {collider1.Tag} colisionó con {collider2.Tag}");
            }
            else if (!isColliding && wasColliding)
            {
                OnCollisionSeparated?.Invoke(entity1, entity2);
                _previousCollisions[key] = false;
            }
        }

        private (int, int) GetCollisionKey(int id1, int id2) 
            => id1 < id2 ? (id1, id2) : (id2, id1);

        private bool CheckAABBCollision(Transform t1, ColliderComponent c1, Transform t2, ColliderComponent c2)
        {
            // AABB (Axis-Aligned Bounding Box) collision
            float left1 = t1.X - c1.Width / 2f;
            float right1 = t1.X + c1.Width / 2f;
            float top1 = t1.Y - c1.Height / 2f;
            float bottom1 = t1.Y + c1.Height / 2f;

            float left2 = t2.X - c2.Width / 2f;
            float right2 = t2.X + c2.Width / 2f;
            float top2 = t2.Y - c2.Height / 2f;
            float bottom2 = t2.Y + c2.Height / 2f;

            // ← Margen para evitar problemas de floating point
            const float MARGIN = 0.1f;

            return !(right1 < left2 + MARGIN || left1 > right2 - MARGIN ||
                bottom1 < top2 + MARGIN || top1 > bottom2 - MARGIN);
        }

        /// <summary>
        /// Resuelve colisiones sólidas (movimiento bloqueado).
        /// </summary>
        public bool CanMoveTo(World world, Entity entity, float newX, float newY)
        {
            if (!world.HasComponent<ColliderComponent>(entity))
                return true;

            var entityCollider = world.GetComponent<ColliderComponent>(entity);

            // Si no es sólido, no bloquea nada
            if (!entityCollider.IsSolid)
                return true;

            var testTransform = new Transform(newX, newY);

            var obstacles = world.GetEntitiesWith<Transform, ColliderComponent>().ToList();

            foreach (var obstacle in obstacles)
            {
                if (obstacle.Id == entity.Id) continue;
                if (!obstacle.IsValid) continue;

                var obsCollider = world.GetComponent<ColliderComponent>(obstacle);
                if (!obsCollider.IsSolid) continue;

                var obsTransform = world.GetComponent<Transform>(obstacle);

                if (CheckAABBCollision(testTransform, entityCollider, obsTransform, obsCollider))
                {
                    return false; // Movimiento bloqueado
                }
            }

            return true; // Movimiento permitido
        }

        

        public void SetDebugMode(bool enabled) => _debugMode = enabled;
    }
}
