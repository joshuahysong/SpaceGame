using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Ships;
using System;
using System.Collections.Generic;

namespace SpaceGame
{
    public class MainGame : Game
    {
        public static MainGame Instance { get; private set; }
        public static Camera Camera { get; set; }
        public static ShipBase Player { get; set; }
        public static bool IsDebugging { get; set; }

        public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
        public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);
        public static Vector2 ScreenCenter => new Vector2(Viewport.Width / 2, Viewport.Height / 2);

        public const int WorldTileSize = 4000;

        public Dictionary<string, string> PlayerDebugEntries { get; set; }
        public Dictionary<string, string> SystemDebugEntries { get; set; }

        private Texture2D _debugTile;
        private SpriteBatch _spriteBatch;

        public MainGame()
        {
            var graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = 1080,
                PreferredBackBufferWidth = 1920,
            };
            Window.IsBorderless = true;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            Instance = this;
            IsDebugging = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Camera = new Camera();
            PlayerDebugEntries = new Dictionary<string, string>();
            SystemDebugEntries = new Dictionary<string, string>();
            _debugTile = Art.CreateRectangle(WorldTileSize, WorldTileSize, Color.Transparent, Color.DimGray * 0.5f);

            base.Initialize();

            Player = new TestPlayerShip(Vector2.Zero, 0);
            Camera.Focus = Player;
            EntityManager.Add(Player);

            for (var i = 1; i <= 2; i++)
            {
                var enemy = new Enemy(new TestEnemyShip(new Vector2(i * 100, (i % 2 == 0 ? 1 : -1) * 100), 0));
                EntityManager.Add(enemy);
            }
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
                Input.Update();
                HandleInput();
                Camera.HandleInput();
            }

            Camera.Update(Camera.Focus);
            EntityManager.Update(gameTime, Matrix.Identity);
            base.Update(gameTime);

            if (IsDebugging)
            {
                SystemDebugEntries["FPS"] = $"{Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds)}";
                SystemDebugEntries["Camera Focus"] = $"{Camera.Focus.GetType().Name}";
                SystemDebugEntries["Camera Zoom"] = $"{Math.Round(Camera.Scale, 2)}";
                SystemDebugEntries["Mouse Screen Position"] = $"{Input.ScreenMousePosition.X}, {Input.ScreenMousePosition.Y}";
                SystemDebugEntries["Mouse World Position"] = $"{Math.Round(Input.WorldMousePosition.X)}, {Math.Round(Input.WorldMousePosition.Y)}";
                SystemDebugEntries["Entities"] = $"{EntityManager.Count}";
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, Camera.Transform);
            DrawBackground(_spriteBatch);
            EntityManager.Draw(_spriteBatch, Matrix.Identity);
            _spriteBatch.End();

            if (IsDebugging)
            {
                _spriteBatch.Begin();
                var xTextOffset = 5;
                var yTextOffset = 5;
                _spriteBatch.DrawString(Art.DebugFont, "Player", new Vector2(xTextOffset, yTextOffset), Color.White);
                foreach (KeyValuePair<string, string> debugEntry in PlayerDebugEntries)
                {
                    yTextOffset += 15;
                    _spriteBatch.DrawString(Art.DebugFont, $"{debugEntry.Key}: {debugEntry.Value}", new Vector2(xTextOffset, yTextOffset), Color.White);
                }

                yTextOffset = 5;
                foreach (KeyValuePair<string, string> debugEntry in SystemDebugEntries)
                {
                    var text = $"{debugEntry.Key}: {debugEntry.Value}";
                    xTextOffset = (int)(Viewport.Width - 5 - Art.DebugFont.MeasureString(text).X);
                    _spriteBatch.DrawString(Art.DebugFont, text, new Vector2(xTextOffset, yTextOffset), Color.White);
                    yTextOffset += 15;
                }
                _spriteBatch.End();
            }

            base.Draw(gameTime);
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
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            Vector2 startLocation = Vector2.Zero;
            int numberOfTilesX = (int)Math.Ceiling((double)Viewport.Bounds.Width / WorldTileSize / Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)Viewport.Bounds.Height / WorldTileSize / Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(Camera.Position.X / WorldTileSize);
            tilePosition.Y = (int)Math.Floor(Camera.Position.Y / WorldTileSize);

            startLocation.X = startLocation.X - WorldTileSize;

            int minX = (int)Math.Floor(tilePosition.X - (double)numberOfTilesX / 2);
            int maxX = (int)Math.Ceiling(tilePosition.X + (double)numberOfTilesX / 2);
            int minY = (int)Math.Floor(tilePosition.Y - (double)numberOfTilesY / 2);
            int maxY = (int)Math.Ceiling(tilePosition.Y + (double)numberOfTilesY / 2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Rectangle backgroundRectangle = new Rectangle(WorldTileSize * x, WorldTileSize * y, WorldTileSize, WorldTileSize);
                    spriteBatch.Draw(Art.Background, backgroundRectangle, Color.White);

                    Vector2 position = new Vector2(_debugTile.Bounds.Width * x, _debugTile.Bounds.Height * y);
                    if (IsDebugging && Camera.Scale >= 0.5)
                    {
                        _spriteBatch.Draw(_debugTile, position, Color.White);
                        _spriteBatch.DrawString(Art.DebugFont, $"{x},{y}", new Vector2(position.X + 5, position.Y + 5), Color.DimGray);
                    }
                }
            }
        }
    }
}