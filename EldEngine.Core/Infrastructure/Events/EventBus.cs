using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EldEngine.Core.Infrastructure.Events
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EventBus mejorado con soporte para EventBase
    /// ═════════════════════════════════════════════════════════════════════════

    public partial class EventBus: IDisposable
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
        private readonly Queue<(Type, IGameEvent)> _eventQueue = new();
        private bool _isProcessing = false;
        private int _eventCount = 0;
        public event Action<string> OnEventPublished;

        /// <summary>
        /// Publica un evento con validación automática.
        /// </summary>
        public void PublishValidated<T>(T @event) where T : EventBase
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            if (!@event.IsValid())
                throw new InvalidOperationException(
                    $"Evento {typeof(T).Name} no es válido");

            Publish(@event);
        }

        /// <summary>
        /// Obtiene estadísticas de eventos por tipo.
        /// </summary>
        public string GetEventStats()
        {
            var stats = string.Join(", ",
                _subscribers.Keys.Select(t => $"{t.Name}({GetSubscriberCount(t)})"));

            return $"EventBus | Tipos: {_subscribers.Count} | [{stats}]";
        }

        /// <summary>
        /// Suscribe un handler a un tipo de evento.
        /// </summary>
        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);

            if (!_subscribers.ContainsKey(eventType))
                _subscribers[eventType] = new List<Delegate>();

            _subscribers[eventType].Add(handler);

            Debug.WriteLine($"[EventBus] ✓ Suscriptor registrado para {eventType.Name}");
        }

        /// <summary>
        /// Desuscribe un handler de un tipo de evento.
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType].Remove(handler);

                if (_subscribers[eventType].Count == 0)
                    _subscribers.Remove(eventType);

                Debug.WriteLine($"[EventBus] ✓ Suscriptor removido de {eventType.Name}");
            }
        }

        /// <summary>
        /// Publica un evento - Se encola para procesar después de frame.
        /// Evita modificar colecciones durante iteración.
        /// </summary>
        public void Publish<T>(T @event) where T : IGameEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            _eventQueue.Enqueue((typeof(T), @event));
            _eventCount++;

            Debug.WriteLine($"[EventBus] 📤 Evento encolado: {@event.EventName}");
        }

        /// <summary>
        /// Procesa todos los eventos encolados.
        /// Debe llamarse una vez por frame (después de actualizar lógica).
        /// </summary>
        public void ProcessEvents()
        {
            if (_isProcessing)
            {
                Debug.WriteLine("[EventBus] ⚠️ Ya está procesando eventos, ignorando...");
                return;
            }

            _isProcessing = true;

            try
            {
                int processed = 0;

                while (_eventQueue.Count > 0)
                {
                    var (eventType, @event) = _eventQueue.Dequeue();

                    if (_subscribers.ContainsKey(eventType))
                    {
                        var handlers = _subscribers[eventType];

                        foreach (var handler in handlers)
                        {
                            try
                            {
                                ((dynamic)handler)(@event);
                                processed++;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(
                                    $"[EventBus] ❌ Error procesando evento {eventType.Name}: {ex.Message}");
                            }
                        }
                    }

                    OnEventPublished?.Invoke(@event.EventName);
                }

                if (processed > 0)
                {
                    Debug.WriteLine($"[EventBus] ✓ {processed} eventos procesados");
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        /// <summary>
        /// Obtiene cantidad de suscriptores para un evento (debugging).
        /// </summary>
        public int GetSubscriberCount<T>() where T : IGameEvent
        {
            var eventType = typeof(T);
            return _subscribers.ContainsKey(eventType) ? _subscribers[eventType].Count : 0;
        }

        /// <summary>
        /// Limpia todos los eventos y suscriptores.
        /// </summary>
        public void Clear()
        {
            _eventQueue.Clear();
            _subscribers.Clear();
            _eventCount = 0;
            Debug.WriteLine("[EventBus] 🧹 Bus limpiado");
        }

        public void Dispose()
        {
            Clear();
        }

        public override string ToString()
        {
            var subscriberInfo = string.Join(", ",
                _subscribers.Keys.Select(t => $"{t.Name}({_subscribers[t].Count})"));

            return $"EventBus | Eventos totales: {_eventCount} | Suscriptores: [{subscriberInfo}]";
        }

        private int GetSubscriberCount(Type eventType)
        {
            return _subscribers.ContainsKey(eventType) ?
                _subscribers[eventType].Count : 0;
        }
    }
}
