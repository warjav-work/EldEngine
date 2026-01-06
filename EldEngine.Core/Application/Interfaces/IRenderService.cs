namespace EldEngine.Core.Application.Interfaces
{
    public interface IRenderService : IDisposable
    {
        void Initialize(int width, int height, string title);
        void BeginFrame();
        void EndFrame();
        void Clear(float r = 0, float g = 0, float b = 0);

        void DrawSprite(float x, float y, string assetPath, float rotation = 0);
        void DrawRectangle(float x, float y, float width, float height, uint color);
        void DrawText(string text, float x, float y, float size, uint color);

        bool IsRunning { get; }
    }
}
