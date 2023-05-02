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
        private Dictionary<string, string> _playerDebugEntries = new();
        private Dictionary<string, string> _systemDebugEntries = new();

        private Player _player;
        private Texture2D _starTile1;
        private Texture2D _starTile2;
        private Texture2D _starTile3;
        private bool _isPaused;

        public void Setup()
        {
            EntityManager.Initialize();
            CollisionManager.Initialize();
            ParticleEffectsManager.Initialize();

            _player = new Player(new TestShip1(FactionType.Player, Vector2.Zero, 0));
            MainGame.Camera.Focus = _player;
            EntityManager.Add(_player);

            _starTile1 = GetStarsTexture(1000);
            _starTile2 = GetStarsTexture(1000);
            _starTile3 = GetStarsTexture(1000);

            for (var i = 1; i <= 1; i++)
            {
                var enemy = new Enemy(
                    new TestShip2(FactionType.Enemy, new Vector2(i * 200, (i % 2 == 0 ? 1 : -1) * 200), 0),
                    _player.Ship);
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
                _playerDebugEntries["Position"] = $"{Math.Round(_player.Ship.Position.X)}, {Math.Round(_player.Ship.Position.Y)}";
                _playerDebugEntries["Velocity"] = $"{Math.Round(_player.Ship.Velocity.X)}, {Math.Round(_player.Ship.Velocity.Y)}";
                _playerDebugEntries["Heading"] = $"{Math.Round(_player.Ship.Heading, 2)}";
                _playerDebugEntries["Velocity Heading"] = $"{Math.Round(_player.Ship.Velocity.ToAngle(), 2)}";
                _playerDebugEntries["Current Turn Rate"] = $"{Math.Round(_player.Ship.CurrentTurnRate, 2)}";
                _systemDebugEntries["Mouse World Position"] = $"{Math.Round(Input.WorldMousePosition.X)}, {Math.Round(Input.WorldMousePosition.Y)}";
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.BlueNebula1, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, null, null, null, MainGame.Camera.Transform);
            DrawStarTiles(spriteBatch, _starTile1, Color.White, 0.8f);
            DrawStarTiles(spriteBatch, _starTile2, Color.White, 0.5f);
            DrawStarTiles(spriteBatch, _starTile3, Color.White, 0.1f);
            spriteBatch.Draw(Art.Planet.RedPlanet, Vector2.Zero, Color.White);
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
                if (_playerDebugEntries.Any())
                {
                    spriteBatch.DrawString(Art.DebugFont, "Player", new Vector2(5, yTextOffset), Color.White);
                    foreach (KeyValuePair<string, string> debugEntry in _playerDebugEntries)
                    {
                        yTextOffset += 15;
                        spriteBatch.DrawString(Art.DebugFont, $"{debugEntry.Key}: {debugEntry.Value}", new Vector2(5, yTextOffset), Color.White);
                    }
                }

                if (_systemDebugEntries.Any())
                {
                    yTextOffset = 110;
                    foreach (KeyValuePair<string, string> debugEntry in _systemDebugEntries)
                    {
                        var text = $"{debugEntry.Key}: {debugEntry.Value}";
                        var xTextOffset = (int)(MainGame.Viewport.Width - 5 - Art.DebugFont.MeasureString(text).X);
                        spriteBatch.DrawString(Art.DebugFont, text, new Vector2(xTextOffset, yTextOffset), Color.White);
                        yTextOffset += 15;
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
            var cameraOffset = parallaxScale == 1f ? parallaxScale : (1f - parallaxScale);
            tilePosition.X = (int)Math.Floor(MainGame.Camera.Position.X * cameraOffset / size);
            tilePosition.Y = (int)Math.Floor(MainGame.Camera.Position.Y * cameraOffset / size);

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
