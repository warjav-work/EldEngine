using EldEngine.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.GameTest.Components
{
    public struct NetworkComponent : IComponent
    {
        static int IComponent.GetComponentTypeId() => typeof(NetworkComponent).GetHashCode();
    }
}
