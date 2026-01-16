using System;
using System.Collections.Generic;
using System.Text;

namespace EldEngine.Core.Infrastructure.Events
{
    /// <summary>
    /// Base abstracta para todos los eventos del juego.
    /// Proporciona funcionalidad común.
    /// </summary>
    public abstract class EventBase : IGameEvent
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Nombre del evento (automático del nombre de la clase).
        /// </summary>
        public virtual string EventName => GetType().Name;

        /// <summary>
        /// Timestamp cuando se creó el evento.
        /// </summary>
        public DateTime CreatedAt { get; private set; } = DateTime.Now;

        /// <summary>
        /// ID único del evento (para tracking).
        /// </summary>
        public Guid EventId { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Prioridad del evento (mayor = procesar primero).
        /// Rango: 0-100
        /// </summary>
        public virtual int Priority => 50;

        /// <summary>
        /// ¿El evento es crítico? (nunca cancelar)
        /// </summary>
        public virtual bool IsCritical => false;

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Valida que el evento tiene datos válidos.
        /// </summary>
        public virtual bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Obtiene información del evento.
        /// </summary>
        public virtual string GetEventInfo()
        {
            var elapsed = (DateTime.Now - CreatedAt).TotalMilliseconds;
            return $"{EventName} (ID: {EventId:N}) - {elapsed:F0}ms ago";
        }

        /// <summary>
        /// Obtiene información del evento.
        /// </summary>
        public override string ToString()
        {
            return GetEventInfo();
        }
    }
}
