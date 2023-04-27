using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Entities;
using SpaceGame.Managers;
using SpaceGame.Ships;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

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
        private Texture2D _starTile1;
        private Texture2D _starTile2;
        private Texture2D _starTile3;
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

            for (var i = 1; i <= 2; i++)
            {
                var enemy = new Enemy(new TestEnemyShip(new Vector2(i * 200, (i % 2 == 0 ? 1 : -1) * 200), 0));
                EntityManager.Add(enemy);
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
            _starTile1 = GetStarsTexture(400);
            _starTile2 = GetStarsTexture(400);
            _starTile3 = GetStarsTexture(400);
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
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            _spriteBatch.Draw(Art.Background, Vector2.Zero, new Rectangle(0, 0, Viewport.Width, Viewport.Height), Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, Camera.Transform);
            //_spriteBatch.Draw(_starTile1,
            //    Vector2.Zero,
            //    Color.White);

            _spriteBatch.Draw(_starTile1,
                //Vector2.Zero,
                Player.Position * 0.5f,
                new Rectangle(0, 0, Viewport.Width, Viewport.Height),
                //new Rectangle((int)Math.Ceiling(Camera.Position.X), (int)Math.Ceiling(Camera.Position.Y), Viewport.Width, Viewport.Height),
                Color.Red, 0f,
                //Vector2.Zero,
                new Vector2(Viewport.Width / 2, Viewport.Height / 2),
                1f, SpriteEffects.None, 0f);
            _spriteBatch.End();

            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, Camera.GetMatrix(new Vector2(0.5f)));
            //DrawStarTiles(_starTile1, Color.Red);
            //_spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, Camera.Transform);
            DrawStarTiles(_starTile2, Color.Yellow);
            if (IsDebugging)
            {
                DrawDebugTiles();
            }
            EntityManager.Draw(_spriteBatch, Matrix.Identity);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred);
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

        private void DrawDebugTiles()
        {
            Vector2 startLocation = Vector2.Zero;
            int numberOfTilesX = (int)Math.Ceiling((double)Viewport.Bounds.Width / TileSize / Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)Viewport.Bounds.Height / TileSize / Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(Camera.Position.X / TileSize);
            tilePosition.Y = (int)Math.Floor(Camera.Position.Y / TileSize);

            startLocation.X -= TileSize;

            int minX = (int)Math.Floor(tilePosition.X - (double)numberOfTilesX / 2);
            int maxX = (int)Math.Ceiling(tilePosition.X + (double)numberOfTilesX / 2);
            int minY = (int)Math.Floor(tilePosition.Y - (double)numberOfTilesY / 2);
            int maxY = (int)Math.Ceiling(tilePosition.Y + (double)numberOfTilesY / 2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 position = new Vector2(_debugTile.Bounds.Width * x, _debugTile.Bounds.Height * y);
                    if (Camera.Scale >= 0.5)
                    {
                        _spriteBatch.Draw(_debugTile, position, Color.DimGray * 0.5f);
                        _spriteBatch.DrawString(Art.DebugFont, $"{x},{y}", new Vector2(position.X + 5, position.Y + 5), Color.DimGray * 0.5f);
                    }
                }
            }
        }

        private Texture2D GetStarsTexture(int size)
        {
            var texture = new Texture2D(GraphicsDevice, size, size);
            var Random = new Random();
            var data = new Color[size * size];
            for (int i = 0; i < data.Length; ++i)
            {
                if ((i < size || i % size == 0 || i > size * size - size || (i + 1) % size == 0))
                {
                    data[i] = Color.White;
                }
                if (Random.Next(1, size * 10) == 1)
                {
                    data[i] = Color.White;
                }
            }
            texture.SetData(data);
            return texture;
        }

        private void DrawStarTiles(Texture2D texture, Color color)
        {
            var size = texture.Width > texture.Height ? texture.Width : texture.Height;
            float scale = (float)TileSize / size;
            Vector2 startLocation = Vector2.Zero;
            int numberOfTilesX = (int)Math.Ceiling((double)Viewport.Bounds.Width / size / Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)Viewport.Bounds.Height / size / Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(Camera.Position.X / size);
            tilePosition.Y = (int)Math.Floor(Camera.Position.Y / size);

            startLocation.X = startLocation.X - size;

            int minX = (int)Math.Floor(tilePosition.X - (double)numberOfTilesX / 2);
            int maxX = (int)Math.Ceiling(tilePosition.X + (double)numberOfTilesX / 2);
            int minY = (int)Math.Floor(tilePosition.Y - (double)numberOfTilesY / 2);
            int maxY = (int)Math.Ceiling(tilePosition.Y + (double)numberOfTilesY / 2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 position = new Vector2(_starTile1.Bounds.Width * x, _starTile1.Bounds.Height * y);
                    _spriteBatch.Draw(_starTile1, position, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);

                    //    _spriteBatch.Draw(_starTile1, position,
                    //new Rectangle(0, 0, _starTile1.Width, _starTile1.Height), Color.White,
                    //0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

                    //_spriteBatch.Draw(_starTile2, position, new Rectangle(
                    //    (int)Math.Floor(Camera.Position.X * 0.25f),
                    //    (int)Math.Floor(Camera.Position.Y * 0.25f),
                    //    _starTile1.Width,
                    //    _starTile1.Height), Color.Yellow);
                    //_spriteBatch.Draw(_starTile3, position, new Rectangle(
                    //    (int)Math.Floor(Camera.Position.X * 0.1f),
                    //    (int)Math.Floor(Camera.Position.Y * 0.1f),
                    //    _starTile1.Width,
                    //    _starTile1.Height), Color.Green);
                }
            }
        }
    }
}