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
        private int _universeRadius;
        private Dictionary<string, SolarSystem> _solarSystemNameLookup;
        private List<(Vector2, Vector2)> _linesToDraw;
        private SpriteFont _solarSystemFont;

        public UniverseMapScene()
        {
            _camera = new Camera();
            _universeRadius = 1000;
            GenerateSolarSystems();
            _solarSystemNameLookup = _solarSystems.ToDictionary(x => x.Name, y => y);
            //_linesToDraw = _solarSystems
            //    .Where(x => x.NeighborsByName.All(y => _solarSystemNameLookup.ContainsKey(y)))
            //    .SelectMany(x => x.NeighborsByName,
            //        (x, y) => (x.MapLocation, _solarSystemNameLookup[y].MapLocation))
            //    .Distinct()
            //    .ToList();
            _solarSystemFont = Art.Fonts.UIMediumFont;
            MainGame.SetCurrentSolarSystem(_solarSystems.First());
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
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Locked to screen
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.Backgrounds.BlueNebula1, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.75f);
            spriteBatch.End();

            // Locked to world
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, _camera.GetTransform(MainGame.ScreenCenter));
            //foreach (var lineToDraw in _linesToDraw)
            //{
            //    Art.DrawLine(spriteBatch, lineToDraw.Item1, lineToDraw.Item2, Color.Gray, 2);
            //}
            foreach (var solarSystem in _solarSystems)
            {
                spriteBatch.DrawCircle(solarSystem.MapLocation, 8, 32, Color.Gray, 3);
                //spriteBatch.DrawCircle(solarSystem.MapLocation, 5, 32, Color.Black, 10);
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
                _camera.Position = _camera.Position.Y < -_universeRadius / 2
                    ? new Vector2(_camera.Position.X, -_universeRadius / 2)
                    : new Vector2(_camera.Position.X, _camera.Position.Y - cameraSpeed);
            }
            if (Input.WasKeyPressed(Keys.S) || Input.IsKeyPressed(Keys.S))
            {
                _camera.Position = _camera.Position.Y > _universeRadius / 2
                    ? new Vector2(_camera.Position.X, _universeRadius / 2)
                    : new Vector2(_camera.Position.X, _camera.Position.Y + cameraSpeed);
            }
            if (Input.WasKeyPressed(Keys.A) || Input.IsKeyPressed(Keys.A))
            {
                _camera.Position = _camera.Position.X < -_universeRadius / 2
                    ? new Vector2(-_universeRadius / 2, _camera.Position.Y)
                    : new Vector2(_camera.Position.X - cameraSpeed, _camera.Position.Y);
            }
            if (Input.WasKeyPressed(Keys.D) || Input.IsKeyPressed(Keys.D))
            {
                _camera.Position = _camera.Position.X > _universeRadius / 2
                    ? new Vector2(_universeRadius / 2, _camera.Position.Y)
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
                GenerateSolarSystems();
            }
        }

        private void GenerateSolarSystems()
        {
            var random = new Random();
            var systemsToAdd = 100;
            var tempName = 0;
            var minimumDistance = 50;
            _solarSystems = new List<SolarSystem>();
            while (_solarSystems.Count < systemsToAdd)
            {
                var newLocation = GetRandomPoint(random);

                // Check minimum distance (This is not a gross implementation but works)
                var closeNeighborFound = false;
                foreach (var mapLocation in _solarSystems.Select(x => x.MapLocation))
                {
                    if (Vector2.Distance(newLocation, mapLocation) < minimumDistance)
                    {
                        closeNeighborFound = true;
                        break;
                    }
                }

                if (closeNeighborFound)
                    continue;

                var index = random.Next(0, Art.Backgrounds.All.Count - 1);
                var background = Art.Backgrounds.All.ToArray()[index];
                var newSystem = new SolarSystem(
                    tempName.ToString(),
                    FactionType.None,
                    new Vector2(newLocation.X, newLocation.Y),
                    null,
                    CreateTestPlanet(random),
                    background);

                _solarSystems.Add(newSystem);
                tempName++;
            }
        }

        private Vector2 GetRandomPoint(Random random)
        {
            var angle = random.NextDouble() * Math.PI * 2;
            var radius = Math.Sqrt(random.NextDouble()) * _universeRadius;
            var x = 0 + radius * Math.Cos(angle);
            var y = 0 + radius * Math.Sin(angle);
            return new Vector2((float)x, (float)y);
        }

        private List<Planet> CreateTestPlanet(Random random)
        {
            var index = random.Next(0, Art.Planets.All.Count - 1);
            var texture = Art.Planets.All.ToArray()[index];

            // TODO Generated text
            var description = new List<string>();
            //description.Add($"The sun beats down on the empty spaceport, its harsh rays reflecting off the sand dunes." +
            //    $"The only sound is the wind whistling through the abandoned buildings." +
            //    $"The spaceport is a reminder of a time when this planet was thriving, but now it is nothing more than a ghost town.");
            //description.Add($"The once-busy spaceport is now deserted. The docking bays are empty, the hangars are closed, and the control tower is silent." +
            //    $"The only signs of life are the occasional sand lizards that scuttle across the tarmac.");
            //description.Add($"The spaceport is a relic of a bygone era." +
            //    $"It was once a hub of commerce and trade, but now it is nothing more than a monument to a lost civilization. The buildings are crumbling," +
            //    $"the ships are rusting, and the sand is slowly encroaching on the tarmac.");
            //description.Add($"The spaceport is a reminder of the fragility of life." +
            //    $"It is a reminder that even the most prosperous civilizations can be brought to ruin. It is a reminder that nothing lasts forever.");

            description.Add($"Some interesting description here...");
            return new List<Planet> { new Planet(FactionType.None, Vector2.Zero, texture, description) };
        }
    }
}
