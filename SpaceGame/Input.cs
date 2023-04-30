using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SpaceGame;
using System.Collections.Generic;

namespace SpaceGame
{
    public static class Input
    {
        public static List<Keys> ManagedKeys { get; set; }
        public static Vector2 ScreenMousePosition { get; private set; }
        public static Vector2 WorldMousePosition { get; private set; }
        public static int MouseScrollWheelValue { get; private set; }

        private static MouseState _mouseState;
        private static KeyboardState _keyboardState;
        private static KeyboardState _lastKeyboardState;
        private static MouseState _lastMouseState;
        private static GamePadState _gamepadState;
        private static GamePadState _lastGamepadState;

        public static void Update(Camera camera)
        {
            ManagedKeys = new List<Keys>();
            _lastKeyboardState = _keyboardState;
            _lastMouseState = _mouseState;
            _lastGamepadState = _gamepadState;

            _keyboardState = Keyboard.GetState();
            _mouseState = Mouse.GetState();
            _gamepadState = GamePad.GetState(PlayerIndex.One);

            ScreenMousePosition = new Vector2(_mouseState.X, _mouseState.Y);
            WorldMousePosition = Vector2.Transform(_mouseState.Position.ToVector2(), Matrix.Invert(camera.Transform));
            MouseScrollWheelValue = _mouseState.ScrollWheelValue;
        }

        public static bool IsKeyPressed(Keys key)
        {
            return _keyboardState.IsKeyDown(key);
        }

        public static bool WasKeyPressed(Keys key)
        {
            return _lastKeyboardState.IsKeyUp(key) && _keyboardState.IsKeyDown(key);
        }

        public static bool IsKeyToggled(Keys key)
        {
            return !_lastKeyboardState.IsKeyDown(key) && _keyboardState.IsKeyDown(key);
        }

        public static bool IsButtonPressed(Buttons button)
        {
            return _gamepadState.IsButtonDown(button);
        }

        public static bool WasButtonPressed(Buttons button)
        {
            return _lastGamepadState.IsButtonUp(button) && _gamepadState.IsButtonDown(button);
        }

        public static bool WasLeftMouseButtonClicked()
        {
            return _lastMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed;
        }
    }
}