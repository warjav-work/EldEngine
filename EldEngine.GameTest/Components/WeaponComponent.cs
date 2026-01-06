using EldEngine.Core.Domain.Entities;

namespace EldEngine.GameTest.Components
{
    public struct WeaponComponent : IComponent
    {
        public string WeaponName { get; set; }
        public int Damage { get; set; }
        public float AttackSpeed { get; set; }
        public float Range { get; set; }

        public WeaponComponent(string name, int damage, float speed = 1f)
        {
            WeaponName = name;
            Damage = damage;
            AttackSpeed = speed;
            Range = 2f;
        }

        static int IComponent.GetComponentTypeId() => typeof(WeaponComponent).GetHashCode();
    }
}
