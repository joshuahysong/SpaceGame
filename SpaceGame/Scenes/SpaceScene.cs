using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Entities;
using SpaceGame.Managers;
using SpaceGame.Ships;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Scenes
{
    public class SpaceScene : IScene
    {
        public Player Player { get; private set; }

        public const int TileSize = 400;
        public Dictionary<string, string> PlayerDebugEntries = new();
        public Dictionary<string, string> SystemDebugEntries = new();

        private Texture2D _debugTile;
        private Texture2D _starTile1;
        private Texture2D _starTile2;
        private bool _isPaused;

        public void Setup()
        {
            EntityManager.Initialize();
            CollisionManager.Initialize();
            ParticleEffectsManager.Initialize();

            Player = new Player(new TestShip1(FactionType.Player, Vector2.Zero, 0));
            MainGame.Camera.Focus = Player;
            EntityManager.Add(Player);

            _starTile1 = GetStarsTexture(1000);
            _starTile2 = GetStarsTexture(1000);
            _debugTile = Art.CreateRectangleTexture(TileSize, TileSize, Color.Transparent, Color.WhiteSmoke * ScaleType.Half);

            for (var i = 1; i <= 1; i++)
            {
                var enemy = new Enemy(
                    new TestShip2(FactionType.Enemy, new Vector2(i * 200, (i % 2 == 0 ? 1 : -1) * 200), 0),
                    Player.Ship);
                EntityManager.Add(enemy);
            }

            //var dummy = new Dummy(new TestShip2(FactionType.Enemy, new Vector2(200, 0), (float)(Math.PI / 2f)));
            //EntityManager.Add(dummy);
        }

        public void Update(GameTime gameTime)
        {
            if (MainGame.Instance.IsActive)
            {
                HandleInput();
            }

            if (_isPaused)
                return;

            EntityManager.Update(gameTime, Matrix.Identity);
            CollisionManager.Update();
            ParticleEffectsManager.Update(gameTime, Matrix.Identity);

            if (MainGame.IsDebugging)
            {
                PlayerDebugEntries["Position"] = $"{Math.Round(Player.Ship.Position.X)}, {Math.Round(Player.Ship.Position.Y)}";
                PlayerDebugEntries["Velocity"] = $"{Math.Round(Player.Ship.Velocity.X)}, {Math.Round(Player.Ship.Velocity.Y)}";
                PlayerDebugEntries["Heading"] = $"{Math.Round(Player.Ship.Heading, 2)}";
                PlayerDebugEntries["Velocity Heading"] = $"{Math.Round(Player.Ship.Velocity.ToAngle(), 2)}";
                PlayerDebugEntries["World Tile"] = $"{Player.TileCoordinates.X}, {Player.TileCoordinates.Y}";
                PlayerDebugEntries["Current Turn Rate"] = $"{Math.Round(Player.Ship.CurrentTurnRate, 2)}";
                SystemDebugEntries["Mouse World Position"] = $"{Math.Round(Input.WorldMousePosition.X)}, {Math.Round(Input.WorldMousePosition.Y)}";
                SystemDebugEntries["Mouse World Tile"] = $"{Math.Floor(Input.WorldMousePosition.X / TileSize)}, {Math.Floor(Input.WorldMousePosition.Y / TileSize)}";
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.Background, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, MainGame.Camera.Transform);
            DrawStarTiles(spriteBatch, _starTile1, Color.White, ScaleType.Half);
            DrawStarTiles(spriteBatch, _starTile2, Color.White);
            if (MainGame.IsDebugging)
            {
                DrawDebugTiles(spriteBatch);
            }
            EntityManager.Draw(spriteBatch, Matrix.Identity);
            ParticleEffectsManager.Draw(spriteBatch, Matrix.Identity);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred);
            var fpsText = $"FPS: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds)}";
            var fpsX = (int)(MainGame.Viewport.Width - 5 - Art.DebugFont.MeasureString(fpsText).X);
            spriteBatch.DrawString(Art.DebugFont, fpsText, new Vector2(fpsX, 5), Color.White);
            DrawDebug(spriteBatch);
            spriteBatch.End();
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            if (MainGame.IsDebugging)
            {
                var yTextOffset = 5;
                if (PlayerDebugEntries.Any())
                {
                    spriteBatch.DrawString(Art.DebugFont, "Player", new Vector2(5, yTextOffset), Color.White);
                    foreach (KeyValuePair<string, string> debugEntry in PlayerDebugEntries)
                    {
                        yTextOffset += 15;
                        spriteBatch.DrawString(Art.DebugFont, $"{debugEntry.Key}: {debugEntry.Value}", new Vector2(5, yTextOffset), Color.White);
                    }
                }

                if (SystemDebugEntries.Any())
                {
                    yTextOffset = 110;
                    foreach (KeyValuePair<string, string> debugEntry in SystemDebugEntries)
                    {
                        var text = $"{debugEntry.Key}: {debugEntry.Value}";
                        var xTextOffset = (int)(MainGame.Viewport.Width - 5 - Art.DebugFont.MeasureString(text).X);
                        spriteBatch.DrawString(Art.DebugFont, text, new Vector2(xTextOffset, yTextOffset), Color.White);
                        yTextOffset += 15;
                    }
                }
            }
        }

        private void DrawDebugTiles(SpriteBatch spriteBatch)
        {
            int numberOfTilesX = (int)Math.Ceiling((double)MainGame.Viewport.Bounds.Width / TileSize / MainGame.Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)MainGame.Viewport.Bounds.Height / TileSize / MainGame.Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(MainGame.Camera.Position.X / TileSize);
            tilePosition.Y = (int)Math.Floor(MainGame.Camera.Position.Y / TileSize);

            int minX = (int)Math.Floor(tilePosition.X - (double)numberOfTilesX / 2);
            int maxX = (int)Math.Ceiling(tilePosition.X + (double)numberOfTilesX / 2);
            int minY = (int)Math.Floor(tilePosition.Y - (double)numberOfTilesY / 2);
            int maxY = (int)Math.Ceiling(tilePosition.Y + (double)numberOfTilesY / 2);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 position = new Vector2(_debugTile.Bounds.Width * x, _debugTile.Bounds.Height * y);
                    if (MainGame.Camera.Scale >= 0.5)
                    {
                        spriteBatch.Draw(_debugTile, position, Color.DimGray * 0.5f);
                        spriteBatch.DrawString(Art.DebugFont, $"{x},{y}", new Vector2(position.X + 5, position.Y + 5), Color.DimGray * 0.5f);
                    }
                }
            }
        }

        private Texture2D GetStarsTexture(int size, bool drawBorder = false)
        {
            var texture = new Texture2D(MainGame.Instance.GraphicsDevice, size, size);
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

        private void DrawStarTiles(SpriteBatch spriteBatch, Texture2D texture, Color color, float parallaxScale = 1f)
        {
            var size = texture.Width > texture.Height ? texture.Width : texture.Height;
            int numberOfTilesX = (int)Math.Ceiling((double)MainGame.Viewport.Bounds.Width / size / MainGame.Camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)MainGame.Viewport.Bounds.Height / size / MainGame.Camera.Scale);
            Vector2 tilePosition;
            tilePosition.X = (int)Math.Floor(MainGame.Camera.Position.X * parallaxScale / size);
            tilePosition.Y = (int)Math.Floor(MainGame.Camera.Position.Y * parallaxScale / size);

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
                        position += MainGame.Camera.Position * parallaxScale;
                    }
                    spriteBatch.Draw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
        }

        private void HandleInput()
        {
            if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
            {
                var scene = new PauseMenuScene();
                scene.Setup();
                MainGame.SetScene(scene);
            }
            if (Input.WasKeyPressed(Keys.Pause))
            {
                _isPaused = !_isPaused;
            }
        }
    }
}
