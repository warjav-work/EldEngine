using EldEngine.Core.Domain.Values;
using System.Drawing;
using System.IO;

namespace EldEngine.Core.Application.Services
{
    public class SpriteAssetManager
    {
        private Dictionary<string, SpriteSheet> spritesheets;
        private Dictionary<string, Bitmap> imageCache;
        private string assetsPath;

        public SpriteAssetManager(string assetsPath = "./Assets/Sprites/")
        {
            this.assetsPath = assetsPath;
            spritesheets = new Dictionary<string, SpriteSheet>();
            imageCache = new Dictionary<string, Bitmap>();

            // Crear directorio si no existe
            Directory.CreateDirectory(assetsPath);
        }

        /// <summary>
        /// Cargar imagen desde archivo (con caché)
        /// </summary>
        public Bitmap LoadImage(string imagePath)
        {
            if (imageCache.ContainsKey(imagePath))
                return imageCache[imagePath];

            string fullPath = Path.Combine(assetsPath, imagePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException(
                    $"Imagen no encontrada: {fullPath}");

            var image = new Bitmap(fullPath);
            imageCache[imagePath] = image;

            Console.WriteLine($"[SpriteAssetManager] Imagen cargada: {imagePath}");
            return image;
        }

        /// <summary>
        /// Registrar spritesheet manualmente
        /// </summary>
        public void RegisterSpriteSheet(SpriteSheet spriteSheet)
        {
            spritesheets[spriteSheet.Id] = spriteSheet;
            Console.WriteLine(
                $"[SpriteAssetManager] SpriteSheet registrado: {spriteSheet.Id}");
        }

        /// <summary>
        /// Cargar spritesheet desde archivo PNG + JSON
        /// </summary>
        public SpriteSheet LoadSpriteSheet(string spritesheetId,
            string imagePath, int tileWidth, int tileHeight)
        {
            var image = LoadImage(imagePath);
            var spriteSheet = SpriteSheet.CreateFromGrid(
                spritesheetId, image, tileWidth, tileHeight);

            RegisterSpriteSheet(spriteSheet);
            return spriteSheet;
        }

        /// <summary>
        /// Cargar spritesheet desde archivo JSON personalizado
        /// </summary>
        public SpriteSheet LoadSpriteSheetFromJson(string spritesheetId,
            string imagePath, string jsonMetadataPath)
        {
            var image = LoadImage(imagePath);
            var spriteSheet = new SpriteSheet(
                spritesheetId, image, 0, 0);

            // TODO: Parsear JSON y cargar frames personalizados
            // Ejemplo con Newtonsoft.Json:
            /*
            string fullJsonPath = Path.Combine(assetsPath, jsonMetadataPath);
            string jsonContent = File.ReadAllText(fullJsonPath);
            var metadata = JsonConvert.DeserializeObject<SpritesheetMetadata>(jsonContent);

            foreach (var anim in metadata.Animations)
            {
                spriteSheet.AddAnimation(anim.Name, anim.Frames);
            }
            */

            RegisterSpriteSheet(spriteSheet);
            return spriteSheet;
        }

        /// <summary>
        /// Obtener spritesheet por ID
        /// </summary>
        public SpriteSheet GetSpriteSheet(string spritesheetId)
        {
            if (!spritesheets.ContainsKey(spritesheetId))
                throw new KeyNotFoundException(
                    $"SpriteSheet '{spritesheetId}' no registrado");

            return spritesheets[spritesheetId];
        }

        /// <summary>
        /// Obtener frame específico
        /// </summary>
        public Frame GetFrame(string spritesheetId, string animationName,
            int frameIndex)
        {
            var sheet = GetSpriteSheet(spritesheetId);
            return sheet.GetFrame(animationName, frameIndex);
        }

        /// <summary>
        /// Limpiar caché
        /// </summary>
        public void ClearCache()
        {
            foreach (var image in imageCache.Values)
                image?.Dispose();

            foreach (var sheet in spritesheets.Values)
                sheet?.Dispose();

            imageCache.Clear();
            spritesheets.Clear();

            Console.WriteLine("[SpriteAssetManager] Caché limpiado");
        }

        /// <summary>
        /// Listar todos los spritesheets cargados (debug)
        /// </summary>
        public void DebugListSheets()
        {
            Console.WriteLine("═══ SPRITESHEETS CARGADOS ═══");
            foreach (var sheet in spritesheets.Values)
            {
                Console.WriteLine($"  • {sheet.Id}");
            }
            Console.WriteLine($"Total: {spritesheets.Count}");
        }
    }
}
