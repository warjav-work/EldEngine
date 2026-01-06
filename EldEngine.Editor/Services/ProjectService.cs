using System.IO;
using System.Text.Json;

namespace EldEngine.Editor.Services
{
    /// <summary>
    /// Gestiona proyectos de editor (directorio, archivos, configuración).
    /// </summary>
    public class ProjectService
    {
        private string _projectPath;
        private string _scenesPath;
        private string _assetsPath;

        public string ProjectPath => _projectPath;
        public string ScenesPath => _scenesPath;
        public string AssetsPath => _assetsPath;

        public void CreateProject(string projectName, string basePath)
        {
            _projectPath = Path.Combine(basePath, projectName);
            _scenesPath = Path.Combine(_projectPath, "Scenes");
            _assetsPath = Path.Combine(_projectPath, "Assets");

            // Crear estructura de directorios
            Directory.CreateDirectory(_scenesPath);
            Directory.CreateDirectory(Path.Combine(_assetsPath, "Sprites"));
            Directory.CreateDirectory(Path.Combine(_assetsPath, "Sounds"));
            Directory.CreateDirectory(Path.Combine(_assetsPath, "Fonts"));

            // Crear archivo de configuración
            CreateProjectConfig();
        }

        public void OpenProject(string projectPath)
        {
            if (!Directory.Exists(projectPath))
                throw new DirectoryNotFoundException($"Proyecto no encontrado: {projectPath}");

            _projectPath = projectPath;
            _scenesPath = Path.Combine(_projectPath, "Scenes");
            _assetsPath = Path.Combine(_projectPath, "Assets");
        }

        private void CreateProjectConfig()
        {
            var config = new
            {
                projectName = Path.GetFileName(_projectPath),
                version = "1.0",
                engineVersion = "1.0",
                scenes = new string[] { },
                assetPaths = new[]
                {
                    "Assets/Sprites",
                    "Assets/Sounds",
                    "Assets/Fonts"
                }
            };

            var configPath = Path.Combine(_projectPath, "project.config");
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }
    }
}
