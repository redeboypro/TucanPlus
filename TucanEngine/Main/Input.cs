using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;

namespace TucanEngine.Main
{
    public static class Input
    {
        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";

        private static Dictionary<string, (Key, Key)> inputAxes = new Dictionary<string, (Key, Key)> 
        {
            { VerticalAxis, (Key.S, Key.W) },
            { HorizontalAxis, (Key.D, Key.A) }
        };
        private static KeyboardState keyboardState = Keyboard.GetState();
        private static MouseState mouseState = Mouse.GetState();
        private static Vector2 lastMouseLocation;
        private static float mouseDeltaX;
        private static float mouseDeltaY;
        
        public static int GetAxis(string axisName) {
            return Convert.ToInt32(IsKeyDown(inputAxes[axisName].Item2)) - Convert.ToInt32(IsKeyDown(inputAxes[axisName].Item1));
        }

        public static bool IsKeyDown(Key key) {
            return keyboardState.IsKeyDown(key);
        }
        
        public static bool IsKeyRelease(Key key) {
            return keyboardState.IsKeyUp(key);
        }
        
        public static bool IsMouseButtonDown(MouseButton button) {
            return mouseState.IsButtonDown(button);
        }
        
        public static bool IsMouseButtonUp(MouseButton button) {
            return mouseState.IsButtonUp(button);
        }
        
        public static bool IsAnyKeyDown() {
            return keyboardState.IsAnyKeyDown;
        }
        
        public static bool IsAnyMouseButtonDown() {
            return mouseState.IsAnyButtonDown;
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