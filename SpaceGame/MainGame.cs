using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Entities;
using SpaceGame.Managers;
using SpaceGame.Ships;
using System;
using System.Collections.Generic;

namespace SpaceGame
{
    public class MainGame : Game
    {
        public static MainGame Instance { get; private set; }
        public static Camera Camera { get; set; }
        public static Player Player { get; set; }
        public static bool IsDebugging { get; set; }

        public static Viewport Viewport => Instance.GraphicsDevice.Viewport;
        public static Vector2 ScreenSize => new Vector2(Viewport.Width, Viewport.Height);
        public static Vector2 ScreenCenter => new Vector2(Viewport.Width / 2, Viewport.Height / 2);

        public const int TileSize = 400;

        public Dictionary<string, string> PlayerDebugEntries { get; set; }
        public Dictionary<string, string> EnemyDebugEntries { get; set; }
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
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            Instance = this;
            IsDebugging = false;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Camera = new Camera();
            PlayerDebugEntries = new Dictionary<string, string>();
            EnemyDebugEntries = new Dictionary<string, string>();
            SystemDebugEntries = new Dictionary<string, string>();
            _debugTile = Art.CreateRectangle(TileSize, TileSize, Color.Transparent, Color.WhiteSmoke * 0.5f);

            base.Initialize();

            Player = new Player(new TestPlayerShip(Vector2.Zero, 0));
            Camera.Focus = Player;
            EntityManager.Add(Player);

            for (var i = 1; i <= 100; i++)
            {
                var enemy = new Enemy(new TestEnemyShip(new Vector2(i * 200, (i % 2 == 0 ? 1 : -1) * 200), 0));
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
            CollisionManager.Update();
            base.Update(gameTime);

            if (IsDebugging)
            {
                SystemDebugEntries["Camera Focus"] = $"{Camera.Focus.GetType().Name}";
                SystemDebugEntries["Camera Zoom"] = $"{Math.Round(Camera.Scale, 2)}";
                SystemDebugEntries["Mouse Screen Position"] = $"{Input.ScreenMousePosition.X}, {Input.ScreenMousePosition.Y}";
                SystemDebugEntries["Mouse World Position"] = $"{Math.Round(Input.WorldMousePosition.X)}, {Math.Round(Input.WorldMousePosition.Y)}";
                SystemDebugEntries["Mouse World Tile"] = $"{Math.Floor(Input.WorldMousePosition.X / TileSize)}, {Math.Floor(Input.WorldMousePosition.Y / TileSize)}";
                SystemDebugEntries["Entities"] = $"{EntityManager.Count}";
                SystemDebugEntries["Collidables"] = $"{CollisionManager.Count}";
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, Camera.Transform);
            DrawBackground(_spriteBatch);
            if (IsDebugging)
            {
                DrawDebugTiles();
            }
            EntityManager.Draw(_spriteBatch, Matrix.Identity);
            _spriteBatch.End();

            _spriteBatch.Begin();
            var fpsText = $"FPS: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds)}";
            var fpsX = (int)(Viewport.Width - 5 - Art.DebugFont.MeasureString(fpsText).X);
            _spriteBatch.DrawString(Art.DebugFont, fpsText, new Vector2(fpsX, 5), Color.White);
            if (IsDebugging)
            {
                var xTextOffset = 5;
                var yTextOffset = 5;
                _spriteBatch.DrawString(Art.DebugFont, "Player", new Vector2(xTextOffset, yTextOffset), Color.White);
                foreach (KeyValuePair<string, string> debugEntry in PlayerDebugEntries)
                {
                    yTextOffset += 15;
                    _spriteBatch.DrawString(Art.DebugFont, $"{debugEntry.Key}: {debugEntry.Value}", new Vector2(xTextOffset, yTextOffset), Color.White);
                }

                xTextOffset = Viewport.Width / 2 - 200;
                yTextOffset = 5;
                _spriteBatch.DrawString(Art.DebugFont, "Ship", new Vector2(xTextOffset, yTextOffset), Color.White);
                foreach (KeyValuePair<string, string> debugEntry in EnemyDebugEntries)
                {
                    yTextOffset += 15;
                    _spriteBatch.DrawString(Art.DebugFont, $"{debugEntry.Key}: {debugEntry.Value}", new Vector2(xTextOffset, yTextOffset), Color.White);
                }

                yTextOffset = 20;
                foreach (KeyValuePair<string, string> debugEntry in SystemDebugEntries)
                {
                    var text = $"{debugEntry.Key}: {debugEntry.Value}";
                    xTextOffset = (int)(Viewport.Width - 5 - Art.DebugFont.MeasureString(text).X);
                    _spriteBatch.DrawString(Art.DebugFont, text, new Vector2(xTextOffset, yTextOffset), Color.White);
                    yTextOffset += 15;
                }
            }
            _spriteBatch.End();

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
            var backgroundImage = Art.Background;
            var backgroundTileSize = 2 * (backgroundImage.Width > backgroundImage.Height ? backgroundImage.Width : backgroundImage.Height);
            Vector2 startLocation = Vector2.Zero;
            int numberOfTilesX = (int)Math.Ceiling((double)Viewport.Bounds.Width / backgroundTileSize / Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)Viewport.Bounds.Height / backgroundTileSize / Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(Camera.Position.X / backgroundTileSize);
            tilePosition.Y = (int)Math.Floor(Camera.Position.Y / backgroundTileSize);

            startLocation.X = startLocation.X - backgroundTileSize;

            int minX = (int)Math.Floor(tilePosition.X - (double)numberOfTilesX / 2);
            int maxX = (int)Math.Ceiling(tilePosition.X + (double)numberOfTilesX / 2);
            int minY = (int)Math.Floor(tilePosition.Y - (double)numberOfTilesY / 2);
            int maxY = (int)Math.Ceiling(tilePosition.Y + (double)numberOfTilesY / 2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Rectangle backgroundRectangle = new Rectangle(backgroundTileSize * x, backgroundTileSize * y, backgroundTileSize, backgroundTileSize);
                    spriteBatch.Draw(Art.Background, backgroundRectangle, Color.White);
                }
            }
        }

        private void DrawDebugTiles()
        {
            Vector2 startLocation = Vector2.Zero;
            int numberOfTilesX = (int)Math.Ceiling((double)Viewport.Bounds.Width / TileSize / Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)Viewport.Bounds.Height / TileSize / Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(Camera.Position.X / TileSize);
            tilePosition.Y = (int)Math.Floor(Camera.Position.Y / TileSize);

            startLocation.X = startLocation.X - TileSize;

            int minX = (int)Math.Floor(tilePosition.X - (double)numberOfTilesX / 2);
            int maxX = (int)Math.Ceiling(tilePosition.X + (double)numberOfTilesX / 2);
            int minY = (int)Math.Floor(tilePosition.Y - (double)numberOfTilesY / 2);
            int maxY = (int)Math.Ceiling(tilePosition.Y + (double)numberOfTilesY / 2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 position = new Vector2(_debugTile.Bounds.Width * x, _debugTile.Bounds.Height * y);
                    if (IsDebugging && Camera.Scale >= 0.5)
                    {
                        _spriteBatch.Draw(_debugTile, position, Color.DimGray * 0.5f);
                        _spriteBatch.DrawString(Art.DebugFont, $"{x},{y}", new Vector2(position.X + 5, position.Y + 5), Color.DimGray * 0.5f);
                    }
                }
            }
        }
    }
}