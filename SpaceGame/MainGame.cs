using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame
{
    public class MainGame : Game
    {
        public static MainGame Instance { get; private set; }
        public static bool IsDebugging { get; private set; }
        public static RenderTarget2D RenderTarget { get; private set; }

        public Camera Camera { get; private set; }

        public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
        public static Vector2 ScreenCenter => new(Viewport.Width / 2, Viewport.Height / 2);

        private Dictionary<string, string> _systemDebugEntries = new();

        private SpriteBatch _spriteBatch;
        private static IScene _previousScene;
        private static IScene _currentScene;
        private static GraphicsDeviceManager _graphics;
        private int _windowWidth;
        private int _windowHeight;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = 1080,
                PreferredBackBufferWidth = 1920,
                SynchronizeWithVerticalRetrace = false
            };
            _graphics.ApplyChanges();

            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            Content.RootDirectory = "Content";
            Instance = this;
            IsDebugging = false;
            IsMouseVisible = true;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            Camera = new Camera();
            RenderTarget = new RenderTarget2D(GraphicsDevice, 6000, 6000);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
            _currentScene = new MainMenuScene();
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
                HandleInput();

            if (_currentScene != null)
                _currentScene.Update(gameTime);

            if (IsDebugging)
            {
                _systemDebugEntries["Mouse Screen Position"] = $"{Input.ScreenMousePosition.X}, {Input.ScreenMousePosition.Y}";
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_currentScene != null)
                _currentScene.Draw(gameTime, _spriteBatch);

            DrawDebug(gameTime);

            base.Draw(gameTime);
        }

        public static void SetScene(IScene scene)
        {
            if (_currentScene != null)
                _previousScene = _currentScene;

            _currentScene = scene;
        }

        public static void SwitchToPreviousScene()
        {
            _currentScene = _previousScene;
            _previousScene = null;
        }

        private void DrawDebug(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            var fpsText = $"FPS: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds)}";
            var fpsX = (int)(Viewport.Width - 5 - Art.Fonts.DebugFont.MeasureString(fpsText).X);
            _spriteBatch.DrawString(Art.Fonts.DebugFont, fpsText, new Vector2(fpsX, 5), Color.White);
            if (IsDebugging && _systemDebugEntries.Any())
            {
                var yTextOffset = 20;
                foreach (KeyValuePair<string, string> debugEntry in _systemDebugEntries)
                {
                    var text = $"{debugEntry.Key}: {debugEntry.Value}";
                    var xTextOffset = (int)(Viewport.Width - 5 - Art.Fonts.DebugFont.MeasureString(text).X);
                    _spriteBatch.DrawString(Art.Fonts.DebugFont, text, new Vector2(xTextOffset, yTextOffset), Color.White);
                    yTextOffset += 15;
                }
            }
            _spriteBatch.End();
        }

        private void HandleInput()
        {
            if (Input.WasKeyPressed(Keys.F4))
            {
                IsDebugging = !IsDebugging;
            }
            if ((Input.IsKeyPressed(Keys.LeftAlt) || Input.IsKeyPressed(Keys.RightAlt)) && Input.WasKeyPressed(Keys.Enter))
            {
                if (_graphics.IsFullScreen)
                {
                    _graphics.PreferredBackBufferWidth = _windowWidth;
                    _graphics.PreferredBackBufferHeight = _windowHeight;
                    _graphics.HardwareModeSwitch = false;
                    _graphics.IsFullScreen = false;
                }
                else
                {
                    _windowWidth = Window.ClientBounds.Width;
                    _windowHeight = Window.ClientBounds.Height;
                    _graphics.IsFullScreen = true;
                }
                _graphics.ApplyChanges();
            }
        }
    }
}