using OpenTK;

namespace TucanEngine.Common.EventTranslation
{
    public static class Ortho
    {
        public static Vector2 ToGlCoordinates(int x, int y) {
            var display = Display.Display.GetCurrent();
            Vector2 processedCoordinates;
            processedCoordinates.X = 2.0f * x / display.Width - 1;
            processedCoordinates.Y = -(2.0f * y / display.Height - 1);
            return processedCoordinates;
        }
        
        public static Vector2 ToScreenCoordinates(float x, float y) {
            var display = Display.Display.GetCurrent();
            Vector2 processedCoordinates;
            processedCoordinates.X = (x + 1) * display.Width / 2;
            processedCoordinates.Y = (y + 1) * display.Height / 2;
            return processedCoordinates;
        }
    }
}