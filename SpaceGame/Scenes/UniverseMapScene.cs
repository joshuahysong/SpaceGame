using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SpaceGame.Scenes.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Scenes
{
    public class UniverseMapScene : IScene
    {
        public string Name => SceneNames.UniverseMap;

        private Dictionary<string, string> _systemDebugEntries = new();
        private List<SolarSystem> _solarSystems = new();

        private Camera _camera;
        private float _cameraMaxY;
        private float _cameraMaxX;
        private Dictionary<string, SolarSystem> _solarSystemNameLookup;
        private List<(Vector2, Vector2)> _linesToDraw;
        private SpriteFont _solarSystemFont;

        public UniverseMapScene(List<SolarSystem> solarSystems)
        {
            _camera = new Camera();
            _solarSystems = solarSystems;
            _solarSystemNameLookup = _solarSystems.ToDictionary(x => x.Name, y => y);
            _linesToDraw = _solarSystems
                .Where(x => x.NeighborsByName != null && x.NeighborsByName.All(y => _solarSystemNameLookup.ContainsKey(y)))
                .SelectMany(x => x.NeighborsByName,
                    (x, y) => (x.MapLocation, _solarSystemNameLookup[y].MapLocation))
                .Distinct()
                .ToList();
            _solarSystemFont = Art.Fonts.UIMediumFont;
            _camera.Position = MainGame.CurrentSolarSystem.MapLocation;
            _cameraMaxY = _solarSystems.Max(x => x.MapLocation.Y);
            _cameraMaxX = _solarSystems.Max(x => x.MapLocation.X);
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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, _camera.GetTransform(MainGame.ScreenCenter));
            foreach (var lineToDraw in _linesToDraw)
            {
                Art.DrawLine(spriteBatch, lineToDraw.Item1, lineToDraw.Item2, Color.Gray, 2);
            }
            foreach (var solarSystem in _solarSystems)
            {
                spriteBatch.DrawCircle(solarSystem.MapLocation, 8, 32, Color.Gray, 3);
                spriteBatch.DrawCircle(solarSystem.MapLocation, 5, 32, Color.Black, 10);
            }
            foreach (var solarSystem in _solarSystems)
            {
                var textSize = _solarSystemFont.MeasureString(solarSystem.Name);
                var textLocation = solarSystem.MapLocation - new Vector2(textSize.X / 2, textSize.Y + 22);
                spriteBatch.DrawString(_solarSystemFont, solarSystem.Name, textLocation, Color.White);
            }
            spriteBatch.End();

            spriteBatch.Begin();
            DrawDebug(spriteBatch);
            spriteBatch.End();
        }

        private void DrawDebug(SpriteBatch spriteBatch)
        {
            if (MainGame.IsDebugging)
            {
                if (_systemDebugEntries.Any())
                {
                    var yTextOffset = 35;
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

        private void HandleInput(GameTime gameTime)
        {
            float cameraSpeed = 400f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
            {
                MainGame.SwitchToScene(SceneNames.PauseMenu);
            }
            if (Input.WasKeyPressed(Keys.M))
            {
                MainGame.SwitchToScene(SceneNames.Space);
            }
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

            if (MainGame.IsDebugging && Input.WasKeyPressed(Keys.R))
            {
                MainGame.DebugRegenerateUniverse();
            }
        }
    }
}
