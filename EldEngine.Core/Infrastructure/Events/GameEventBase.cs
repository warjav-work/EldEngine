namespace EldEngine.Core.Infrastructure.Events
{
    public class GameEventBase : EventBase
    {
        public string GameState { get; set; }
        public float GameTime { get; set; }

        public override int Priority => 50;

        public GameEventBase(string gameState, float gameTime)
        {
            GameState = gameState;
            GameTime = gameTime;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(GameState) && GameTime >= 0;
        }

        public override string GetEventInfo()
        {
            return $"{base.GetEventInfo()} | State: {GameState} | Time: {GameTime:F1}s";
        }
    }
}
