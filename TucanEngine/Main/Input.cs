using OpenTK;
using OpenTK.Input;

namespace TucanEngine.Main
{
    public static class Input
    {
        private static KeyboardState keyboardState = Keyboard.GetState();
        private static MouseState mouseState = Mouse.GetState();
        private static Vector2 lastMouseLocation;
        private static float mouseDeltaX;
        private static float mouseDeltaY;

        public static bool IsKeyDown(Key key) {
            return keyboardState.IsKeyDown(key);
        }
        
        public static bool IsKeyRelease(Key key) {
            return keyboardState.IsKeyUp(key);
        }
        
        public static bool IsAnyKeyDown() {
            return keyboardState.IsAnyKeyDown;
        }

        public static float GetMouseDeltaX() {
            return mouseDeltaX;
        }
        public static float GetMouseDeltaY() {
            return mouseDeltaY;
        }

        public static void OnLoad() {
            UpdateLastMouseLocation();
        }

        public static void OnUpdateFrame() { 
            keyboardState = Keyboard.GetState(); 
            mouseState = Mouse.GetState();
            mouseDeltaX = mouseState.X - lastMouseLocation.X;
            mouseDeltaY = mouseState.Y - lastMouseLocation.Y;
            UpdateLastMouseLocation();
        }

        private static void UpdateLastMouseLocation() {
            lastMouseLocation = new Vector2(mouseState.X, mouseState.Y);
        }
    }
}