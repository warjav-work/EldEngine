using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    /// <summary>
    /// Componente que maneja el control del jugador por input.
    /// </summary>
    public struct PlayerController : IComponent
    {
        public float MoveSpeed { get; set; }
        public float DashSpeed { get; set; }
        public float DashCooldown { get; set; }
        public bool CanDash { get; set; }

        public PlayerController(float moveSpeed = 5f)
        {
            MoveSpeed = moveSpeed;
            DashSpeed = 15f;
            DashCooldown = 1f;
            CanDash = true;
        }

        static int IComponent.GetComponentTypeId() => typeof(PlayerController).GetHashCode();
    }
}
