using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.GameTest.Systems
{
    public class ParallelMovementSystem : ISystem
    {
        public string Name => "ParallelMovement";
        public int Priority => 20;

        public void Execute(World world, float deltaTime)
        {
            var entities = world.GetEntitiesWith<Transform, Velocity>().ToList();
            var chunkSize = Math.Max(1, entities.Count / Environment.ProcessorCount);

            Parallel.ForEach(
                Partitioner.Create(entities, true),
                new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                entity =>
                {
                    var transform = world.GetComponent<Transform>(entity);
                    var velocity = world.GetComponent<Velocity>(entity);

                    transform.X += velocity.X * deltaTime;
                    transform.Y += velocity.Y * deltaTime;

                    // Thread-safe update
                    lock (world)
                    {
                        world.RemoveComponent<Transform>(entity);
                        world.AddComponent(entity, transform);
                    }
                });
        }
    }
}
