using EldEngine.Core.Domain.Worlds;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace EldEngine.Core.Domain.Systems
{
    /// <summary>
    /// Interfaz para sistemas que procesan la lógica del juego.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Nombre único del sistema para debugging.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Prioridad de ejecución (menor = primero).
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Ejecuta la lógica del sistema.
        /// </summary>
        void Execute(World world, float deltaTime);
    }
}
