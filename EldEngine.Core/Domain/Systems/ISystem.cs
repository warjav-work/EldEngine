using EldEngine.Core.Domain.Worlds;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace EldEngine.Core.Domain.Systems
{
    /// <summary>
    /// Interfaz para sistemas que procesan la lógica del juego.
    /// Define el contrato mínimo para un sistema ECS.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Nombre único del sistema para debugging.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Prioridad de ejecución (menor = primero).
        /// Rango: 0-200
        /// - 0-20: Input Systems
        /// - 20-50: Movement & Physics
        /// - 50-100: Gameplay Logic
        /// - 100-150: Damage & Death
        /// - 150+: Cleanup
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Ejecuta la lógica del sistema.
        /// Llamado una vez por frame.
        /// </summary>
        void Execute(World world, float deltaTime);
    }
}
