using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Entities;
using SpaceGame.Managers;
using SpaceGame.Scenes.Components;
using SpaceGame.Ships;
using SpaceGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Scenes
{
    public class SpaceScene : IScene
    {
        public string Name => SceneNames.Space;

        private Dictionary<string, string> _playerDebugEntries = new();
        private Dictionary<string, string> _systemDebugEntries = new();

        private Camera _camera;
        private Player _player;
        private Texture2D _starTile1;
        private Texture2D _starTile2;
        private Texture2D _minimapContainer;
        private Button _landingButton;
        private LandingScene _landingScene;
        private bool _isLanded;
        private bool _isPaused;
        private SolarSystem _selectedSolarSystem;
        private float _angleToSelectedSolarSystem;
        private SolarSystem _currentSolarSystem;
        private Dictionary<string, SolarSystem> _solarSystemNameLookup;
        private bool disposedValue;

        public SpaceScene(List<SolarSystem> solarSystems)
        {
            UniverseMapScene.SolarSystemSelectionChanged += HandleSolarSystemSelectionChanged;

            _solarSystemNameLookup = solarSystems.ToDictionary(x => x.Name, y => y);

            EntityManager.Initialize();
            CollisionManager.Initialize();
            ParticleEffectsManager.Initialize();

            var random = new Random();
            var index = random.Next(0, solarSystems.Count - 1);
            var startingSolarSystem = solarSystems.ToArray()[index]?.Name;

            _camera = new Camera();
            _player = new Player(new TestShip1(FactionType.Player, Vector2.Zero, 0), startingSolarSystem);

            _camera.Focus = _player;
            EntityManager.Add(_player);

            _starTile1 = GetStarsTexture(1000);
            _starTile2 = GetStarsTexture(1000);
            _minimapContainer = Art.CreateRectangleTexture(200, 200, Color.Purple, Color.White);

            for (var i = 1; i <= 1; i++)
            {
                var enemy = new Enemy(
                    new TestShip2(FactionType.Enemy, new Vector2(i * 200, (i % 2 == 0 ? 1 : -1) * 200), 0),
                    _player.Ship,
                    startingSolarSystem);
                EntityManager.Add(enemy);
            }

            var buttonTexture = Art.CreateRectangleTexture(250, 40, Color.Black, Color.White);
            _landingButton = new Button(buttonTexture, "Land", TextSize.Small, new Vector2(10, 5), 100, 20, Color.White, LandOnPlanet);
        }

        public void Update(GameTime gameTime)
        {
            if (MainGame.Instance.IsActive)
            {
                Input.Update(_camera);
                HandleInput();
            }

            if (_isPaused) return;
            if (_player.Ship.IsExpired) MainGame.SwitchToScene(SceneNames.GameOver);
            if (_player.Ship.IsJumping && _player.Ship.Velocity.LengthSquared() > Constants.JumpSpeed * Constants.JumpSpeed)
                FinishJumpToSystem();
            if (!_solarSystemNameLookup.TryGetValue(_player.CurrentSolarSystemName, out _currentSolarSystem)) return;

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

            if (_player.Ship.DockableLocation != null) _landingButton.Update();

            if (MainGame.IsDebugging)
            {
                _playerDebugEntries["Position"] = $"{Math.Round(_player.Ship.Position.X)}, {Math.Round(_player.Ship.Position.Y)}";
                _playerDebugEntries["Velocity"] = $"{Math.Round(_player.Ship.Velocity.X)}, {Math.Round(_player.Ship.Velocity.Y)}";
                _playerDebugEntries["Heading"] = $"{Math.Round(_player.Ship.Heading, 2)}";
                _playerDebugEntries["Velocity Heading"] = $"{Math.Round(_player.Ship.Velocity.ToAngle(), 2)}";
                _playerDebugEntries["Current Turn Rate"] = $"{Math.Round(_player.Ship.CurrentTurnRate, 2)}";
                _playerDebugEntries["Current Solar System"] = $"{_player.CurrentSolarSystemName}";
                _playerDebugEntries["Selected Solar System"] = $"{_selectedSolarSystem?.Name}";

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
            if (string.IsNullOrWhiteSpace(_player.CurrentSolarSystemName) || _currentSolarSystem == null) return;

            DrawMinimapTexture(gameTime, spriteBatch);

            // Locked to screen
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(_currentSolarSystem.Background, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.5f);
            spriteBatch.End();

            // Locked to world
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, _camera.GetTransform(MainGame.ScreenCenter));
            DrawStarTiles(spriteBatch, _starTile1, Color.White, 0.8f);
            DrawStarTiles(spriteBatch, _starTile2, Color.White, 0.5f);
            DrawStarTiles(spriteBatch, Art.Backgrounds.Starfield1, Color.White, 0.1f);
            _currentSolarSystem.Draw(gameTime, spriteBatch);
            EntityManager.Draw(spriteBatch, Matrix.Identity, _player.CurrentSolarSystemName);
            ParticleEffectsManager.Draw(spriteBatch, Matrix.Identity, _player.CurrentSolarSystemName);
            spriteBatch.End();

            if (_isLanded)
                _landingScene.Draw(gameTime, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred);
            var minimapX = MainGame.Viewport.Width - 270;
            spriteBatch.Draw(_minimapContainer, new Rectangle(minimapX - 1, 19, 252, 252), Color.White);
            spriteBatch.Draw(MainGame.RenderTarget, new Rectangle(minimapX, 20, 250, 250), Color.White);
            spriteBatch.End();

            // Locked to screen
            spriteBatch.Begin(SpriteSortMode.Deferred);
            if (_player.Ship.DockableLocation != null)
                _landingButton.Draw(spriteBatch);
            DrawDebug(spriteBatch);
            spriteBatch.End();

            if (_player.Ship.IsJumping && _player.Ship.Velocity.LengthSquared() > (Constants.JumpSpeed * 0.98) * (Constants.JumpSpeed * 0.98))
            {
                // Locked to screen
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
                spriteBatch.Draw(Art.Misc.Pixel, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.5f);
                spriteBatch.End();
            }
        }

        public void HandleSolarSystemSelectionChanged(SolarSystem selectedSolarSystem)
        {
            _selectedSolarSystem = selectedSolarSystem;
            _angleToSelectedSolarSystem = (_selectedSolarSystem.MapLocation - _currentSolarSystem.MapLocation).ToAngle();
        }

        private void DrawMinimapTexture(GameTime gameTime, SpriteBatch spriteBatch)
        {
            MainGame.Instance.GraphicsDevice.SetRenderTarget(MainGame.RenderTarget);
            MainGame.Instance.GraphicsDevice.Clear(Color.Black);

            var renderCenter = new Vector2(MainGame.RenderTarget.Width / 2, MainGame.RenderTarget.Height / 2);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, _camera.GetTransform(renderCenter, 1f));
            _currentSolarSystem.Draw(gameTime, spriteBatch, true);
            EntityManager.Draw(spriteBatch, Matrix.Identity, _player.CurrentSolarSystemName, true);
            spriteBatch.End();

            MainGame.Instance.GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            if (MainGame.IsDebugging)
            {
                var yTextOffset = 5;
                if (_playerDebugEntries.Any())
                {
                    spriteBatch.DrawString(Art.Fonts.DebugFont, "Player", new Vector2(5, yTextOffset), Color.White);
                    foreach (KeyValuePair<string, string> debugEntry in _playerDebugEntries)
                    {
                        yTextOffset += 15;
                        spriteBatch.DrawString(Art.Fonts.DebugFont, $"{debugEntry.Key}: {debugEntry.Value}", new Vector2(5, yTextOffset), Color.White);
                    }
                }

                if (_systemDebugEntries.Any())
                {
                    yTextOffset = 35;
                    foreach (KeyValuePair<string, string> debugEntry in _systemDebugEntries)
                    {
                        var text = $"{debugEntry.Key}: {debugEntry.Value}";
                        var xTextOffset = (int)(MainGame.Viewport.Width - 5 - Art.Fonts.DebugFont.MeasureString(text).X);
                        spriteBatch.DrawString(Art.Fonts.DebugFont, text, new Vector2(xTextOffset, yTextOffset), Color.White);
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
                MainGame.SwitchToScene(SceneNames.PauseMenu);
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
                MainGame.SwitchToScene(SceneNames.UniverseMap);
            }
            if (Input.WasKeyPressed(Keys.J))
            {
                StartJumpToSystem();
            }

            if (!_isLanded)
            {
                var zoomStep = 0.1f;
                var maximumZoom = 2f;
                var minimumZoom = 0.5f;

                if (Input.WasMouseScrollValueDecreased())
                {
                    _camera.Scale -= zoomStep;
                    if (_camera.Scale < minimumZoom)
                        _camera.Scale = minimumZoom;
                }
                else if (Input.WasMouseScrollValueIncreased())
                {
                    _camera.Scale += zoomStep;
                    if (_camera.Scale > maximumZoom)
                         _camera.Scale = maximumZoom;
                }
            }
        }

        private void LandOnPlanet()
        {
            var dockable = _player.Ship.DockableLocation;
            _landingScene = new LandingScene(dockable.Description);
            _isLanded = true;
        }

        private void StartJumpToSystem()
        {
            if (_selectedSolarSystem != null)
            {
                _player.Ship.StartJump(_angleToSelectedSolarSystem);
            }
        }

        private void FinishJumpToSystem()
        {
            if (_selectedSolarSystem != null)
            {
                _player.CurrentSolarSystemName = _selectedSolarSystem.Name;
                _player.Ship.FinishJump();
            }
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                UniverseMapScene.SolarSystemSelectionChanged -= HandleSolarSystemSelectionChanged;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
