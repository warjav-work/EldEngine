namespace EldEngine.Core.Infrastructure.Events
{
    /// <summary>
    /// Evento base que todos los eventos deben implementar.
    /// </summary>
    public interface IGameEvent
    {
        string EventName { get; }
    }
}
