using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Entities;
using SpaceGame.Managers;
using SpaceGame.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame
{
    public class MainGame : Game
    {
        public static MainGame Instance { get; private set; }
        public static GameState GameState { get; set; }
        public static Camera Camera { get; set; }
        public static Player Player { get; set; }
        public static bool IsDebugging { get; set; }

        public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
        public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);
        public static Vector2 ScreenCenter => new Vector2(Viewport.Width / 2, Viewport.Height / 2);

        public Dictionary<string, string> SystemDebugEntries = new();

        private SpaceScene _spaceScene;
        private SpriteBatch _spriteBatch;
        private bool _isPaused;

        public MainGame()
        {
            var graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = 1080,
                PreferredBackBufferWidth = 1920,
            };
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            Instance = this;
            IsDebugging = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            GameState = GameState.Space;
            Camera = new Camera();
            SystemDebugEntries = new Dictionary<string, string>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                Input.Update(Camera);
                HandleInput();
            }

            if (_isPaused)
                return;

            if (GameState == GameState.MainMenu)
            {
            }

            if (GameState == GameState.Space)
            {
                if (_spaceScene == null)
                {
                    _spaceScene = new SpaceScene();
                    _spaceScene.Initialize();
                }
                _spaceScene.Update(gameTime, IsActive);
            }

            if (IsDebugging)
            {
                SystemDebugEntries["Camera Focus"] = $"{Camera.Focus?.GetType().Name}";
                SystemDebugEntries["Camera Zoom"] = $"{Math.Round(Camera.Scale, 2)}";
                SystemDebugEntries["Entities"] = $"{EntityManager.Count}";
                SystemDebugEntries["Collidables"] = $"{CollisionManager.Count}";
                SystemDebugEntries["Effects"] = $"{ParticleEffectsManager.Count}";
                SystemDebugEntries["Mouse Screen Position"] = $"{Input.ScreenMousePosition.X}, {Input.ScreenMousePosition.Y}";
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (GameState == GameState.MainMenu)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
                _spriteBatch.Draw(Art.Background, Vector2.Zero, new Rectangle(0, 0, Viewport.Width, Viewport.Height), Color.White);
                _spriteBatch.End();
            }

            if (GameState == GameState.Space)
            {
                if (_spaceScene == null)
                {
                    _spaceScene = new SpaceScene();
                    _spaceScene.Initialize();
                }
                _spaceScene.Draw(gameTime, _spriteBatch);
            }

            DrawDebug(gameTime);

            base.Draw(gameTime);
        }

        private void DrawDebug(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            var fpsText = $"FPS: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds)}";
            var fpsX = (int)(Viewport.Width - 5 - Art.DebugFont.MeasureString(fpsText).X);
            _spriteBatch.DrawString(Art.DebugFont, fpsText, new Vector2(fpsX, 5), Color.White);
            if (IsDebugging && SystemDebugEntries.Any())
            {
                var yTextOffset = 20;
                foreach (KeyValuePair<string, string> debugEntry in SystemDebugEntries)
                {
                    var text = $"{debugEntry.Key}: {debugEntry.Value}";
                    var xTextOffset = (int)(Viewport.Width - 5 - Art.DebugFont.MeasureString(text).X);
                    _spriteBatch.DrawString(Art.DebugFont, text, new Vector2(xTextOffset, yTextOffset), Color.White);
                    yTextOffset += 15;
                }
            }
            _spriteBatch.End();
        }

        private void HandleInput()
        {
            if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
            {
                Exit();
            }
            if (Input.WasKeyPressed(Keys.F4))
            {
                IsDebugging = !IsDebugging;
            }
            if (Input.WasKeyPressed(Keys.D1))
            {
                GameState = GameState.Space;
            }
            if (Input.WasKeyPressed(Keys.D2))
            {
                GameState = GameState.MainMenu;
            }
            if (Input.WasKeyPressed(Keys.Pause) && GameState == GameState.Space)
            {
                _isPaused = !_isPaused;
            }
        }
    }
}