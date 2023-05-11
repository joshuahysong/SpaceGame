using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using SpaceGame.SolarSystems;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Scenes
{
    public class UniverseMapScene : IScene
    {
        private Camera _camera;
        private List<ISolarSystem> _solarSystems;
        private Dictionary<string, ISolarSystem> _solarSystemNameLookup;
        private List<(Vector2, Vector2)> _linesToDraw;
        private SpriteFont _solarSystemFont;

        public UniverseMapScene()
        {
            _camera = new Camera();
            _solarSystems = new List<ISolarSystem>
            {
                new TestSystem1(),
                new TestSystem2()
            };
            _solarSystemNameLookup = _solarSystems.ToDictionary(x => x.Name, y => y);
            _linesToDraw = _solarSystems
                .Where(x => x.NeighborsByName.All(y => _solarSystemNameLookup.ContainsKey(y)))
                .SelectMany(x => x.NeighborsByName,
                    (x, y) => (x.MapLocation, _solarSystemNameLookup[y].MapLocation))
                .Distinct()
                .ToList();
            _solarSystemFont = Art.Fonts.UIMediumFont;
        }

        public void Update(GameTime gameTime)
        {
            if (MainGame.Instance.IsActive)
            {
                Input.Update();
                HandleInput();
            }
            _camera.Update();
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
                var textSize = _solarSystemFont.MeasureString(solarSystem.Name);
                var textLocation = solarSystem.MapLocation - new Vector2(textSize.X / 2, textSize.Y + 22);
                spriteBatch.DrawString(_solarSystemFont, solarSystem.Name, textLocation, Color.White);
            }
            spriteBatch.End();
        }

        private void HandleInput()
        {
            if (Input.WasKeyPressed(Keys.M))
            {
                MainGame.SwitchToPreviousScene();
            }
        }
    }
}
