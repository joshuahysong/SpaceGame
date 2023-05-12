using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Scenes;
using SpaceGame.Scenes.Components;
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
        public static SolarSystem CurrentSolarSystem { get; private set; }

        public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
        public static Vector2 ScreenCenter => new(Viewport.Width / 2, Viewport.Height / 2);

        private Dictionary<string, string> _systemDebugEntries = new();

        private SpriteBatch _spriteBatch;
        private static GraphicsDeviceManager _graphics;
        private int _windowWidth;
        private int _windowHeight;
        private static Dictionary<string, IScene> _scenes;
        private static string _currentSceneName;
        private static string _previousSceneName;

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
            RenderTarget = new RenderTarget2D(GraphicsDevice, 6000, 6000);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);

            _scenes = new Dictionary<string, IScene>
            {
                [SceneNames.MainMenu] = new MainMenuScene(),
                [SceneNames.GameOver] = new GameOverScene(),
                [SceneNames.PauseMenu] = new PauseMenuScene(),
                [SceneNames.Space] = new SpaceScene(),
                [SceneNames.UniverseMap] = new UniverseMapScene()
            };

            _currentSceneName = SceneNames.MainMenu;
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
                HandleInput();

            if (_scenes != null && _scenes.ContainsKey(_currentSceneName))
                _scenes[_currentSceneName].Update(gameTime);

            if (IsDebugging)
            {
                _systemDebugEntries["Mouse Screen Position"] = $"{Input.ScreenMousePosition.X}, {Input.ScreenMousePosition.Y}";
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_scenes != null && _scenes.ContainsKey(_currentSceneName))
                _scenes[_currentSceneName].Draw(gameTime, _spriteBatch);

            DrawDebug(gameTime);

            base.Draw(gameTime);
        }

        public static void SwitchToScene(IScene scene)
        {
            if (scene != null &&_scenes != null && _scenes.ContainsKey(scene.Name))
                _scenes[scene.Name] = scene;

            SwitchToScene(scene.Name);
        }

        public static void SwitchToPreviousScene()
        {
            _currentSceneName = _previousSceneName;
            _previousSceneName = null;
        }

        public static void SwitchToScene(string sceneName)
        {
            if (_scenes != null && _scenes.ContainsKey(sceneName))
            {
                _previousSceneName = _currentSceneName;
                _currentSceneName = sceneName;
            }
        }

        public static void SetCurrentSolarSystem(SolarSystem solarSystem)
        {
            if (solarSystem != null)
                CurrentSolarSystem = solarSystem;
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