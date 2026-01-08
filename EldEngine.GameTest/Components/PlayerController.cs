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

        public PlayerController(float moveSpeed = 80f)
        {
            MoveSpeed = moveSpeed;
            DashSpeed = 2f;     // Multiplicador de velocidad
            DashCooldown = 0f;  // Cooldown actual
            CanDash = true;     // Puede hacer dash
        }

        static int IComponent.GetComponentTypeId() => typeof(PlayerController).GetHashCode();

        public override string ToString() =>
            $"PlayerController(Speed: {MoveSpeed} px/s, CanDash: {CanDash})";
    }
}
