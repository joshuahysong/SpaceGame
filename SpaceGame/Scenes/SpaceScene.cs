using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Entities;
using SpaceGame.Managers;
using SpaceGame.Scenes.Components;
using SpaceGame.Ships;
using SpaceGame.SolarSystems;
using SpaceGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Scenes
{
    public class SpaceScene : IScene
    {
        private Dictionary<string, string> _playerDebugEntries = new();
        private Dictionary<string, string> _systemDebugEntries = new();

        private Camera _camera;
        private Player _player;
        private Texture2D _starTile1;
        private Texture2D _starTile2;
        private Texture2D _minimapContainer;
        private Texture2D _viewPortBox;
        private Button _landingButton;
        private ISolarSystem _currentSolarSystem;
        private LandingScene _landingScene;
        private bool _isLanded;
        private bool _isPaused;

        public SpaceScene()
        {
            EntityManager.Initialize();
            CollisionManager.Initialize();
            ParticleEffectsManager.Initialize();

            _camera = new Camera();
            _player = new Player(new TestShip1(FactionType.Player, Vector2.Zero, 0));
            _camera.Focus = _player;
            EntityManager.Add(_player);

            _starTile1 = GetStarsTexture(1000);
            _starTile2 = GetStarsTexture(1000);
            _minimapContainer = Art.CreateRectangleTexture(200, 200, Color.Purple, Color.White);
            _viewPortBox = Art.CreateRectangleTexture(MainGame.Viewport.Bounds.Width, MainGame.Viewport.Bounds.Height, Color.DarkRed * 0.5f, Color.Red);

            _currentSolarSystem = new TestSystem1();

            for (var i = 1; i <= 1; i++)
            {
                var enemy = new Enemy(
                    new TestShip2(FactionType.Enemy, new Vector2(i * 200, (i % 2 == 0 ? 1 : -1) * 200), 0),
                    _player.Ship);
                EntityManager.Add(enemy);
            }

            var buttonTexture = Art.CreateRectangleTexture(250, 40, Color.Black, Color.White);
            _landingButton = new Button(buttonTexture, "Land", TextSize.Small, Vector2.Zero + new Vector2(5, 5), 100, 20, Color.White, LandOnPlanet);
        }

        public void Update(GameTime gameTime)
        {
            if (MainGame.Instance.IsActive)
            {
                Input.Update(_camera);
                HandleInput();
                if (!_isLanded)
                    _camera.HandleInput();
            }

            if (_isPaused)
                return;

            if (_player.Ship.IsExpired)
            {
                MainGame.SetScene(new GameOverScene());
            }

            if (_isLanded)
            {
                _landingScene.Update(gameTime);
                if (_landingScene.IsExiting)
                    _isLanded = false;
                return;
            }

            _camera.Update();

            EntityManager.Update(gameTime, Matrix.Identity);
            CollisionManager.Update();
            ParticleEffectsManager.Update(gameTime, Matrix.Identity);

            if (_player.Ship.DockableLocation != null)
                _landingButton.Update();

            if (MainGame.IsDebugging)
            {
                _playerDebugEntries["Position"] = $"{Math.Round(_player.Ship.Position.X)}, {Math.Round(_player.Ship.Position.Y)}";
                _playerDebugEntries["Velocity"] = $"{Math.Round(_player.Ship.Velocity.X)}, {Math.Round(_player.Ship.Velocity.Y)}";
                _playerDebugEntries["Heading"] = $"{Math.Round(_player.Ship.Heading, 2)}";
                _playerDebugEntries["Velocity Heading"] = $"{Math.Round(_player.Ship.Velocity.ToAngle(), 2)}";
                _playerDebugEntries["Current Turn Rate"] = $"{Math.Round(_player.Ship.CurrentTurnRate, 2)}";

                _systemDebugEntries["Mouse World Position"] = $"{Math.Round(Input.WorldMousePosition.X)}, {Math.Round(Input.WorldMousePosition.Y)}";
                _systemDebugEntries["Camera Focus"] = $"{_camera.Focus?.GetType().Name}";
                _systemDebugEntries["Camera Zoom"] = $"{Math.Round(_camera.Scale, 2)}";
                _systemDebugEntries["Entities"] = $"{EntityManager.Count}";
                _systemDebugEntries["Collidables"] = $"{CollisionManager.Count}";
                _systemDebugEntries["Effects"] = $"{ParticleEffectsManager.Count}";
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawMinimapTexture(gameTime, spriteBatch);

            // Locked to screen
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.Backgrounds.BlueNebula1, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.75f);
            spriteBatch.End();

            // Locked to world
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, _camera.Transform);
            DrawStarTiles(spriteBatch, _starTile1, Color.White, 0.8f);
            DrawStarTiles(spriteBatch, _starTile2, Color.White, 0.5f);
            DrawStarTiles(spriteBatch, Art.Backgrounds.Starfield1, Color.White, 0.1f);
            _currentSolarSystem.Draw(gameTime, spriteBatch);
            EntityManager.Draw(spriteBatch, Matrix.Identity);
            ParticleEffectsManager.Draw(spriteBatch, Matrix.Identity);
            spriteBatch.End();

            if (_isLanded)
                _landingScene.Draw(gameTime, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred);
            spriteBatch.Draw(_minimapContainer, new Rectangle(9, 29, 202, 202), Color.White);
            spriteBatch.Draw(MainGame.RenderTarget, new Rectangle(10, 30, 200, 200), Color.White);
            spriteBatch.End();

            // Locked to screen
            spriteBatch.Begin(SpriteSortMode.Deferred);
            if (_player.Ship.DockableLocation != null)
                _landingButton.Draw(spriteBatch);

            var fpsText = $"FPS: {Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds)}";
            var fpsX = (int)(MainGame.Viewport.Width - 5 - Art.DebugFont.MeasureString(fpsText).X);
            spriteBatch.DrawString(Art.DebugFont, fpsText, new Vector2(fpsX, 5), Color.White);
            DrawDebug(spriteBatch);
            spriteBatch.End();
        }

        private void DrawMinimapTexture(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var screenOrigin = new Vector2(MainGame.Viewport.Width / 2, MainGame.Viewport.Height / 2);
            // Set the render target
            MainGame.Instance.GraphicsDevice.SetRenderTarget(MainGame.RenderTarget);
            //MainGame.Instance.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            MainGame.Instance.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, _camera.TransformMini);
            spriteBatch.Draw(_viewPortBox, _player.WorldPosition, null, Color.White, 0f, screenOrigin, 1 / _camera.Scale, SpriteEffects.None, 1f);
            _currentSolarSystem.DrawMini(gameTime, spriteBatch);
            //EntityManager.Draw(spriteBatch, Matrix.Identity);
            spriteBatch.End();

            // Drop the render target
            MainGame.Instance.GraphicsDevice.SetRenderTarget(null);
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
                    yTextOffset = 35;
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
            int numberOfTilesX = (int)Math.Ceiling((double)MainGame.Viewport.Bounds.Width / size / _camera.Scale);
            int numberOfTilesY = (int)Math.Ceiling((double)MainGame.Viewport.Bounds.Height / size / _camera.Scale);
            Vector2 tilePosition;
            var cameraOffset = parallaxScale == 1f ? parallaxScale : (1f - parallaxScale);
            tilePosition.X = (int)Math.Floor(_camera.Position.X * cameraOffset / size);
            tilePosition.Y = (int)Math.Floor(_camera.Position.Y * cameraOffset / size);

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
                        position += _camera.Position * parallaxScale;
                    }
                    spriteBatch.Draw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
        }

        private void HandleInput()
        {
            if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
            {
                MainGame.SetScene(new PauseMenuScene());
            }
            if (Input.WasKeyPressed(Keys.Pause))
            {
                _isPaused = !_isPaused;
            }
            if (Input.WasKeyPressed(Keys.L) && _player.Ship.DockableLocation != null)
            {
                LandOnPlanet();
            }
            if (Input.WasKeyPressed(Keys.M))
            {
                MainGame.SetScene(new UniverseMapScene());
            }
        }

        private void LandOnPlanet()
        {
            var dockable = _player.Ship.DockableLocation;
            _landingScene = new LandingScene(dockable.Description);
            _isLanded = true;
        }
    }
}
