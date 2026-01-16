namespace EldEngine.Core.Domain.Entities
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// ComponentBase - Clase Abstracta Base
    /// ═════════════════════════════════════════════════════════════════════════
    /// 
    /// VENTAJAS:
    /// ✅ No duplicar GetComponentTypeId() en cada componente
    /// ✅ Propiedades/métodos comunes en un lugar
    /// ✅ Fácil cambiar lógica común sin modificar cada componente
    /// ✅ Type-safe
    ///
    public abstract class ComponentBase : IComponent
    {
        /// <summary>
        /// Implementación por defecto de GetComponentTypeId().
        /// Los componentes hijos pueden sobrescribir si necesitan.
        /// </summary>
        static int IComponent.GetComponentTypeId()
        {
            return typeof(ComponentBase).GetHashCode();
        }

        /// <summary>
        /// Obtiene el tipo de este componente.
        /// </summary>
        public Type GetComponentType()
        {
            return this.GetType();
        }

        /// <summary>
        /// Obtiene el nombre del componente (nombre de la clase).
        /// </summary>
        public string GetComponentName()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Crea una copia profunda del componente.
        /// Útil para copiar entidades.
        /// </summary>
        public virtual ComponentBase Clone()
        {
            // Copia superficial por defecto
            return (ComponentBase)this.MemberwiseClone();
        }

        /// <summary>
        /// Valida que el componente tiene datos válidos.
        /// Se puede sobrescribir para validación personalizada.
        /// </summary>
        public virtual bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Reinicia el componente a su estado por defecto.
        /// Se puede sobrescribir para reseteo personalizado.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Obtiene una descripción legible del componente.
        /// </summary>
        public override string ToString()
        {
            return $"{GetComponentName()}";
        }
    }
}
