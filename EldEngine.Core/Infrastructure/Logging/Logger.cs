namespace EldEngine.Core.Infrastructure.Logging
{
    public static class Logger
    {
        public enum LogLevel { Debug, Info, Warning, Error }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var color = level switch
            {
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Debug => ConsoleColor.Gray,
                _ => ConsoleColor.White
            };

            System.Console.ForegroundColor = color;
            System.Console.WriteLine($"[{timestamp}] [{level}] {message}");
            System.Console.ResetColor();
        }
    }
}
