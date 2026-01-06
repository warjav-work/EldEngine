namespace EldEngine.Core.Domain.Entities
{
    /// <summary>
    /// Identidad única que agrupa componentes. Immutable para thread-safety.
    /// </summary>
    public readonly struct Entity : IEquatable<Entity>
    {
        public readonly int Id { get; }
        public readonly int Version { get; }

        public Entity(int id, int version = 0)
        {
            Id = id;
            Version = version;
        }

        public bool IsValid => Id > 0;

        public override bool Equals(object obj) => obj is Entity e && Equals(e);
        public bool Equals(Entity other) => Id == other.Id && Version == other.Version;
        public override int GetHashCode() => HashCode.Combine(Id, Version);
        public override string ToString() => $"Entity({Id}v{Version})";

        public static bool operator ==(Entity left, Entity right) => left.Equals(right);
        public static bool operator !=(Entity left, Entity right) => !left.Equals(right);
    }
}
