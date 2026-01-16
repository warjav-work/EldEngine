using EldEngine.Core.Domain.Worlds;
using System.Diagnostics;

namespace EldEngine.Core.Application.Services
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// EJEMPLO: Escena que hereda de SceneBase
    /// ═════════════════════════════════════════════════════════════════════════

    public abstract class GameLevelScene : SceneBase
    {
        // Propiedades comunes a todos los niveles de juego
        public int LevelNumber { get; protected set; }
        public int EnemyCount { get; protected set; }
        public int NPCCount { get; protected set; }

        /// <summary>
        /// Implementación base: crear NPCs del nivel.
        /// </summary>
        protected virtual void CreateNPCs(World world)
        {
            Debug.WriteLine($"[{Name}] Creando NPCs...");
        }

        /// <summary>
        /// Implementación base: crear enemigos del nivel.
        /// </summary>
        protected virtual void CreateEnemies(World world)
        {
            Debug.WriteLine($"[{Name}] Creando enemigos...");
        }

        /// <summary>
        /// Implementación base: crear escenario.
        /// </summary>
        protected virtual void CreateEnvironment(World world)
        {
            Debug.WriteLine($"[{Name}] Creando escenario...");
        }

        /// <summary>
        /// Implementación base del flujo de inicialización.
        /// </summary>
        protected sealed override void OnSceneInitialize(World world)
        {
            // Crear en orden
            CreateNPCs(world);
            CreateEnemies(world);
            CreateEnvironment(world);

            // Cada nivel específico puede sobrescribir estos métodos
        }

        protected override void OnSceneCleanup(World world)
        {
            // Limpiar entidades del nivel
            Debug.WriteLine($"[{Name}] Limpiando {EntityCount} entidades");
        }

        public override string GetInfo()
        {
            return $"{base.GetInfo()} | " +
                   $"Level: {LevelNumber} | " +
                   $"Enemies: {EnemyCount} | " +
                   $"NPCs: {NPCCount}";
        }
    }
}
