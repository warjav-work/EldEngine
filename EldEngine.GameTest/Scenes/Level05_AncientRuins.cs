using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Worlds;

namespace EldEngine.GameTest.Scenes
{
    public class Level05_AncientRuins : IScene
    {
        public string Name => "Level_05_AncientRuins";

        public void Initialize(World world)
        {
            System.Diagnostics.Debug.WriteLine("\n╔════════════════════════════════════╗");
            System.Diagnostics.Debug.WriteLine("║  NIVEL 5: RUINAS ANTIGUAS (BOSS)   ║");
            System.Diagnostics.Debug.WriteLine("║  ¡Derrota al Señor de Sombra!      ║");
            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════╝\n");

            // Implementar con boss
            System.Diagnostics.Debug.WriteLine("✓ Nivel 5 cargado");
        }

        public void Cleanup(World world) { }
    }
}
