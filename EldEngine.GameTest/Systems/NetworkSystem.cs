using EldEngine.Core.Domain.Systems;
using EldEngine.Core.Domain.Values;
using EldEngine.Core.Domain.Worlds;
using EldEngine.GameTest.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.GameTest.Systems
{
    public class NetworkSystem : ISystem
    {
        public string Name => "Network";
        public int Priority => 150;

        public void Execute(World world, float deltaTime)
        {
            var networkEntities = world.GetEntitiesWith<NetworkComponent, Transform>();

            foreach (var entity in networkEntities)
            {
                var netComp = world.GetComponent<NetworkComponent>(entity);
                var transform = world.GetComponent<Transform>(entity);

                // Serializar y enviar estado
               /* var state = new EntityState
                {
                    EntityId = entity.Id,
                    X = transform.X,
                    Y = transform.Y
                };

                SendEntityState(state);*/
            }
        }
        /*
        private void SendEntityState(EntityState state)
        {
            // Implementar envío de red
        }*/
    }
}
