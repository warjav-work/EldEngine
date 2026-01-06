namespace EldEngine.Core.Domain.Entities
{
    /// <summary>
    /// Interfaz base para todos los componentes del sistema ECS.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// ID único del tipo de componente para evitar reflexión en runtime.
        /// </summary>
        static abstract int GetComponentTypeId();
    }
}
