using EldEngine.Core.Application.Interfaces;
using EldEngine.Core.Domain.Worlds;

namespace EldEngine.GameTest.Scenes
{
    public class Level04_ElfVillage : IScene
    {
        public string Name => "Level_04_ElfVillage";

        public void Initialize(World world)
        {
            System.Diagnostics.Debug.WriteLine("\n╔════════════════════════════════════╗");
            System.Diagnostics.Debug.WriteLine("║  NIVEL 4: ALDEA ÉLFICA             ║");
            System.Diagnostics.Debug.WriteLine("║  Habla con los aldeanos            ║");
            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════╝\n");

            // Implementar similar a Level01
            System.Diagnostics.Debug.WriteLine("✓ Nivel 4 cargado");
        }

        public void Cleanup(World world) { }
    }
}
