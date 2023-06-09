﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SpaceGame.Entities;
using SpaceGame.Generators;
using SpaceGame.Scenes.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Scenes
{
    public class UniverseMapScene : IScene
    {
        public string Name => SceneNames.UniverseMap;

        public static event Action<SolarSystem> SolarSystemSelectionChanged;

        private Dictionary<string, string> _systemDebugEntries = new();
        private SpriteFont _solarSystemFont = Art.Fonts.UIMediumFont;

        private Camera _camera;
        private float _cameraMaxY;
        private float _cameraMaxX;
        private IEnumerable<(Vector2, Vector2)> _linesToDraw;
        private SolarSystem _selectedSolarSystem;
        private SolarSystem _playerCurrentSolarSystem;
        private bool disposedValue;

        public UniverseMapScene()
        {
            _camera = new Camera();
            _linesToDraw = UniverseGenerator.SolarSystems
                .Where(x => x.NeighborsByName != null && x.NeighborsByName.All(y => UniverseGenerator.SolarSystemLookup.ContainsKey(y)))
                .SelectMany(x => x.NeighborsByName,
                    (x, y) => (x.MapLocation, UniverseGenerator.SolarSystemLookup[y].MapLocation))
                .Distinct();
            _cameraMaxY = UniverseGenerator.SolarSystems.Max(x => x.MapLocation.Y);
            _cameraMaxX = UniverseGenerator.SolarSystems.Max(x => x.MapLocation.X);

            Player.CurrentSolarSystemNameChanged += HandlePlayerCurrentSolarSystemNameChanged;
        }

        public void Update(GameTime gameTime)
        {
            if (MainGame.Instance.IsActive)
            {
                Input.Update(_camera);
                HandleInput(gameTime);
            }
            _camera.Update();

            _systemDebugEntries["Mouse World Position"] = $"{Math.Round(Input.WorldMousePosition.X)}, {Math.Round(Input.WorldMousePosition.Y)}";
            _systemDebugEntries["Camera Zoom"] = $"{Math.Round(_camera.Scale, 2)}";
            _systemDebugEntries["Camera Position"] = $"{Math.Round(_camera.Position.X)}, {Math.Round(_camera.Position.Y)}";
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Locked to screen
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.Backgrounds.BlueNebula1, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.75f);
            spriteBatch.End();

            // Locked to world
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, _camera.GetTransform(MainGame.ScreenCenter));
            foreach (var lineToDraw in _linesToDraw)
            {
                var color = Color.Gray;
                if (_selectedSolarSystem != null && _playerCurrentSolarSystem != null &&
                    ((lineToDraw.Item1 == _playerCurrentSolarSystem.MapLocation && lineToDraw.Item2 == _selectedSolarSystem.MapLocation) ||
                    (lineToDraw.Item1 == _selectedSolarSystem.MapLocation && lineToDraw.Item2 == _playerCurrentSolarSystem.MapLocation)))
                    color = Color.Cyan;
                Art.DrawLine(spriteBatch, lineToDraw.Item1, lineToDraw.Item2, color, 2);
            }
            foreach (var solarSystem in UniverseGenerator.SolarSystems)
            {
                var origin = new Vector2(Art.Misc.SolarSystem.Width / 2, Art.Misc.SolarSystem.Height / 2);
                spriteBatch.Draw(Art.Misc.SolarSystem, solarSystem.MapLocation, null, Color.Gray, 0f, origin, 0.25f, SpriteEffects.None, 1f);
                if (solarSystem == _playerCurrentSolarSystem)
                    spriteBatch.DrawCircle(solarSystem.MapLocation, 12, 32, Color.Yellow, 2);
                else if (solarSystem == _selectedSolarSystem)
                    spriteBatch.DrawCircle(solarSystem.MapLocation, 12, 32, Color.Cyan, 2);
            }
            foreach (var solarSystem in UniverseGenerator.SolarSystems)
            {
                var textSize = _solarSystemFont.MeasureString(solarSystem.Name);
                var textLocation = solarSystem.MapLocation - new Vector2(textSize.X / 2, textSize.Y + 22);
                spriteBatch.DrawString(_solarSystemFont, solarSystem.Name, textLocation, Color.White);
            }
            spriteBatch.End();

            DrawDebug(spriteBatch);
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            if (MainGame.IsDebugging)
            {
                if (_systemDebugEntries.Any())
                {
                    spriteBatch.Begin();
                    var yTextOffset = 35;
                    foreach (KeyValuePair<string, string> debugEntry in _systemDebugEntries)
                    {
                        var text = $"{debugEntry.Key}: {debugEntry.Value}";
                        var xTextOffset = (int)(MainGame.Viewport.Width - 5 - Art.Fonts.DebugFont.MeasureString(text).X);
                        spriteBatch.DrawString(Art.Fonts.DebugFont, text, new Vector2(xTextOffset, yTextOffset), Color.White);
                        yTextOffset += 15;
                    }
                    spriteBatch.End();
                }
            }
        }

        private void HandleInput(GameTime gameTime)
        {
            if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
            {
                MainGame.SwitchToScene(SceneNames.PauseMenu);
            }
            if (Input.WasKeyPressed(Keys.M))
            {
                MainGame.SwitchToScene(SceneNames.Space);
            }
            if (MainGame.IsDebugging && Input.WasKeyPressed(Keys.R))
            {
                MainGame.DebugRegenerateUniverse();
            }

            float cameraSpeed = 400f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Input.WasKeyPressed(Keys.W) || Input.IsKeyPressed(Keys.W))
            {
                _camera.Position = _camera.Position.Y < -_cameraMaxY
                    ? new Vector2(_camera.Position.X, -_cameraMaxY)
                    : new Vector2(_camera.Position.X, _camera.Position.Y - cameraSpeed);
            }
            if (Input.WasKeyPressed(Keys.S) || Input.IsKeyPressed(Keys.S))
            {
                _camera.Position = _camera.Position.Y > _cameraMaxY
                    ? new Vector2(_camera.Position.X, _cameraMaxY)
                    : new Vector2(_camera.Position.X, _camera.Position.Y + cameraSpeed);
            }
            if (Input.WasKeyPressed(Keys.A) || Input.IsKeyPressed(Keys.A))
            {
                _camera.Position = _camera.Position.X < -_cameraMaxX
                    ? new Vector2(-_cameraMaxX, _camera.Position.Y)
                    : new Vector2(_camera.Position.X - cameraSpeed, _camera.Position.Y);
            }
            if (Input.WasKeyPressed(Keys.D) || Input.IsKeyPressed(Keys.D))
            {
                _camera.Position = _camera.Position.X > _cameraMaxX
                    ? new Vector2(_cameraMaxX, _camera.Position.Y)
                    : new Vector2(_camera.Position.X + cameraSpeed, _camera.Position.Y);
            }
            if (Input.IsLeftMouseButtonClicked())
            {
                _camera.Position += Input.PreviousWorldMousePosition - Input.WorldMousePosition;
                if (_camera.Position.Y < -_cameraMaxY) _camera.Position = new Vector2(_camera.Position.X, -_cameraMaxY);
                if (_camera.Position.Y > _cameraMaxY) _camera.Position = new Vector2(_camera.Position.X, _cameraMaxY);
                if (_camera.Position.X < -_cameraMaxX) _camera.Position = new Vector2(-_cameraMaxX, _camera.Position.Y);
                if (_camera.Position.X > _cameraMaxX) _camera.Position = new Vector2(_cameraMaxX, _camera.Position.Y);
            }

            var zoomStep = 0.1f;
            var maximumZoom = 1f;
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

            if (Input.WasLeftMouseButtonClicked())
            {
                foreach (var solarSystem  in UniverseGenerator.SolarSystems)
                {
                    if (Vector2.Distance(Input.WorldMousePosition, solarSystem.MapLocation) <= 16)
                    {
                        _selectedSolarSystem = solarSystem;
                        SolarSystemSelectionChanged?.Invoke(_selectedSolarSystem);
                        break;
                    }
                }
            }
        }

        private void HandlePlayerCurrentSolarSystemNameChanged(string playerCurrentSolarSystemName)
        {
            if (!UniverseGenerator.SolarSystemLookup.TryGetValue(playerCurrentSolarSystemName, out _playerCurrentSolarSystem)) return;
            _camera.Position = _playerCurrentSolarSystem.MapLocation;
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Player.CurrentSolarSystemNameChanged -= HandlePlayerCurrentSolarSystemNameChanged;
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
