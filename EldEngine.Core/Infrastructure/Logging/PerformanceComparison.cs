using System.Diagnostics;

namespace EldEngine.Core.Infrastructure.Logging
{
    public static class PerformanceComparison
    {
        public static void LogSpeedComparison()
        {
            Debug.WriteLine("\n╔════════════════════════════════════════╗");
            Debug.WriteLine("║      SPEED COMPARISON REPORT           ║");
            Debug.WriteLine("╠════════════════════════════════════════╣");

            // Player
            var playerSpeed = 80f;
            var playerFrameDistance = playerSpeed / 60f;
            var playerCrossPantalla = 1024f / playerSpeed;

            Debug.WriteLine($"║ PLAYER:                                ║");
            Debug.WriteLine($"║   Speed: {playerSpeed} px/s                       ║");
            Debug.WriteLine($"║   Per Frame: {playerFrameDistance:F2} px              ║");
            Debug.WriteLine($"║   Cross Screen: {playerCrossPantalla:F2} seconds         ║");

            Debug.WriteLine($"║                                        ║");

            // Enemy Patrol
            var patrolSpeed = 30f;
            var patrolFrameDistance = patrolSpeed / 60f;
            var patrolCrossPantalla = 1024f / patrolSpeed;

            Debug.WriteLine($"║ ENEMY (PATROL):                        ║");
            Debug.WriteLine($"║   Speed: {patrolSpeed} px/s                       ║");
            Debug.WriteLine($"║   Per Frame: {patrolFrameDistance:F2} px              ║");
            Debug.WriteLine($"║   Cross Screen: {patrolCrossPantalla:F2} seconds         ║");

            Debug.WriteLine($"║                                        ║");

            // Enemy Chase
            var chaseSpeed = 50f;
            var chaseFrameDistance = chaseSpeed / 60f;
            var chaseCrossPantalla = 1024f / chaseSpeed;

            Debug.WriteLine($"║ ENEMY (CHASE):                         ║");
            Debug.WriteLine($"║   Speed: {chaseSpeed} px/s                       ║");
            Debug.WriteLine($"║   Per Frame: {chaseFrameDistance:F2} px              ║");
            Debug.WriteLine($"║   Cross Screen: {chaseCrossPantalla:F2} seconds         ║");

            Debug.WriteLine($"║                                        ║");
            Debug.WriteLine($"║ MULTIPLIERS:                           ║");
            Debug.WriteLine($"║   Player vs Patrol: {playerSpeed / patrolSpeed:F1}x       ║");
            Debug.WriteLine($"║   Player vs Chase: {playerSpeed / chaseSpeed:F1}x        ║");
            Debug.WriteLine($"║   Chase vs Patrol: {chaseSpeed / patrolSpeed:F1}x        ║");

            Debug.WriteLine("╚════════════════════════════════════════╝\n");
        }
    }
}
