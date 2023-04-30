﻿using Microsoft.Xna.Framework;
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
        private Texture2D _starTile1;
        private Texture2D _starTile2;
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
            _debugTile = Art.CreateRectangle(TileSize, TileSize, Color.Transparent, Color.WhiteSmoke * ScaleType.Half);

            base.Initialize();

            Player = new Player(new TestShip1(FactionType.Player, Vector2.Zero, 0));
            Camera.Focus = Player;
            EntityManager.Add(Player);

            for (var i = 1; i <= 2; i++)
            {
                var enemy = new Enemy(new TestShip2(FactionType.Enemy, new Vector2(i * 200, (i % 2 == 0 ? 1 : -1) * 200), 0));
                EntityManager.Add(enemy);
            }

            //var dummy = new Dummy(new TestShip2(FactionType.Enemy, new Vector2(200, 0), (float)(Math.PI / 2f)));
            //EntityManager.Add(dummy);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Art.Load(Content);
            _starTile1 = GetStarsTexture(1000);
            _starTile2 = GetStarsTexture(1000);
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
            ParticleEffectsManager.Update(gameTime, Matrix.Identity);

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
                SystemDebugEntries["Effects"] = $"{ParticleEffectsManager.Count}";
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            _spriteBatch.Draw(Art.Background, Vector2.Zero, new Rectangle(0, 0, Viewport.Width, Viewport.Height), Color.White);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, Camera.Transform);
            DrawStarTiles(_starTile1, Color.White, ScaleType.Half);
            DrawStarTiles(_starTile2, Color.White);
            if (IsDebugging)
            {
                DrawDebugTiles();
            }
            EntityManager.Draw(_spriteBatch, Matrix.Identity);
            ParticleEffectsManager.Draw(_spriteBatch, Matrix.Identity);
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.Deferred);
            var fpsText = $"FPS: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds)}";
            var fpsX = (int)(Viewport.Width - 5 - Art.DebugFont.MeasureString(fpsText).X);
            _spriteBatch.DrawString(Art.DebugFont, fpsText, new Vector2(fpsX, 5), Color.White);
            DrawDebug();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawDebug()
        {
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
            int numberOfTilesX = (int)Math.Ceiling((double)Viewport.Bounds.Width / TileSize / Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)Viewport.Bounds.Height / TileSize / Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(Camera.Position.X / TileSize);
            tilePosition.Y = (int)Math.Floor(Camera.Position.Y / TileSize);

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

        private Texture2D GetStarsTexture(int size, bool drawBorder = false)
        {
            var texture = new Texture2D(GraphicsDevice, size, size);
            var Random = new Random();
            var data = new Color[size * size];
            for (int i = 0; i < data.Length; ++i)
            {
                if (drawBorder)
                {
                    if (i < size || i % size == 0 || i > size * size - size || (i + 1) % size == 0)
                    {
                        data[i] = Color.White * 0.5f;
                    }
                }
                var randomResult = Random.Next(1, size * 40);
                if (randomResult == 1)
                {
                    data[i] = Color.White;
                }
                if (randomResult == 2)
                {
                    data[i] = Color.White * 0.5f;
                }
            }
            texture.SetData(data);
            return texture;
        }

        private void DrawStarTiles(Texture2D texture, Color color, float parallaxScale = 1f)
        {
            var size = texture.Width > texture.Height ? texture.Width : texture.Height;
            int numberOfTilesX = (int)Math.Ceiling((double)Viewport.Bounds.Width / size / Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)Viewport.Bounds.Height / size / Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(Camera.Position.X * parallaxScale / size);
            tilePosition.Y = (int)Math.Floor(Camera.Position.Y * parallaxScale / size);

            int minX = (int)Math.Floor(tilePosition.X - (double)numberOfTilesX / 2);
            int maxX = (int)Math.Ceiling(tilePosition.X + (double)numberOfTilesX / 2);
            int minY = (int)Math.Floor(tilePosition.Y - (double)numberOfTilesY / 2);
            int maxY = (int)Math.Ceiling(tilePosition.Y + (double)numberOfTilesY / 2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var position = new Vector2(texture.Bounds.Width * x, texture.Bounds.Height * y);
                    if (parallaxScale != 1f)
                    {
                        position += Camera.Position * parallaxScale;
                    }
                    _spriteBatch.Draw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
        }
    }
}