namespace EldEngine.GameTest
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try
            {
                var gameWindow = new GameWindow();
                Application.Run(gameWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico: {ex.Message}\n{ex.StackTrace}",
                    "EldEngine.GameTest - Error Fatal",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}