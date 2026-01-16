using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Worlds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EldEngine.Core.Application.Services
{
    /// ═════════════════════════════════════════════════════════════════════════
    /// SceneBase - Clase Abstracta Base para Escenas
    /// ═════════════════════════════════════════════════════════════════════════
    /// 
    /// VENTAJAS:
    /// ✅ Estructura consistente para todas las escenas
    /// ✅ Métodos auxiliares comunes (CreateEntity, etc)
    /// ✅ Logging automático
    /// ✅ Ciclo de vida claro
    ///
    public abstract class SceneBase : IScene
    {
        // ═══════════════════════════════════════════════════════════════════
        // PROPIEDADES
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Nombre de la escena.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Descripción de la escena (para debugging).
        /// </summary>
        public virtual string Description => "";

        /// <summary>
        /// Número de entidades creadas en esta escena.
        /// </summary>
        public int EntityCount { get; protected set; } = 0;

        /// <summary>
        /// ¿Está la escena inicializada?
        /// </summary>
        public bool IsInitialized { get; private set; } = false;

        // ═══════════════════════════════════════════════════════════════════
        // EVENTOS
        // ═══════════════════════════════════════════════════════════════════

        public event Action<SceneBase> OnSceneInitialized;
        public event Action<SceneBase> OnSceneCleanupAc;

        // ═══════════════════════════════════════════════════════════════════
        // CICLO DE VIDA
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Inicializa la escena.
        /// </summary>
        public void Initialize(World world)
        {
            if (IsInitialized)
            {
                Debug.WriteLine($"[{Name}] Ya fue inicializado");
                return;
            }

            try
            {
                Debug.WriteLine($"\n╔═════════════════════════════════════╗");
                Debug.WriteLine($"║  INICIALIZANDO ESCENA: {Name.PadRight(20)} ║");
                if (!string.IsNullOrEmpty(Description))
                    Debug.WriteLine($"║  {Description.PadRight(32)}  ║");
                Debug.WriteLine($"╚═════════════════════════════════════╝\n");

                // Llamar a inicialización específica
                OnSceneInitialize(world);

                IsInitialized = true;
                OnSceneInitialized?.Invoke(this);

                Debug.WriteLine($"✓ [{Name}] Escena inicializada ({EntityCount} entidades)\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [{Name}] Error durante inicialización: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Limpia la escena.
        /// </summary>
        public void Cleanup(World world)
        {
            if (!IsInitialized)
                return;

            try
            {
                Debug.WriteLine($"[{Name}] Limpiando escena...");

                // Llamar a limpieza específica
                OnSceneCleanup(world);

                OnSceneCleanupAc?.Invoke(this);

                Debug.WriteLine($"✓ [{Name}] Escena limpiada\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ [{Name}] Error durante limpieza: {ex.Message}");
            }

            IsInitialized = false;
            EntityCount = 0;
        }

        /// <summary>
        /// Método abstracto que cada escena implementa.
        /// Se llama durante Initialize().
        /// </summary>
        protected abstract void OnSceneInitialize(World world);

        /// <summary>
        /// Método abstracto que cada escena implementa.
        /// Se llama durante Cleanup().
        /// </summary>
        protected abstract void OnSceneCleanup(World world);

        // ═══════════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES
        // ═══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Obtiene información de la escena.
        /// </summary>
        public virtual string GetInfo()
        {
            return $"Scene: {Name} | " +
                   $"Entities: {EntityCount} | " +
                   $"Initialized: {IsInitialized}";
        }

        /// <summary>
        /// Obtiene información de la escena.
        /// </summary>
        public override string ToString()
        {
            return GetInfo();
        }
    }
}
