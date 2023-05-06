using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using SpaceGame.SolarSystems;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Scenes
{
    public class UniverseMapScene : IScene
    {
        private Texture2D _solarSystemTexture;
        private Vector2 _solarSystemOrigin;
        private List<ISolarSystem> _solarSystems;
        private Dictionary<string, ISolarSystem> _solarSystemNameLookup;
        private List<(Vector2, Vector2)> _linesToDraw;
        private SpriteFont _solarSystemFont;

        public UniverseMapScene()
        {
            _solarSystemTexture = Art.CreateCircleTexture(10, Color.White);
            _solarSystemOrigin = new Vector2(_solarSystemTexture.Width / 2, _solarSystemTexture.Height / 2);
            _solarSystems = new List<ISolarSystem>
            {
                new TestSystem1(),
                new TestSystem2()
            };
            _solarSystemNameLookup = _solarSystems.ToDictionary(x => x.Name, y => y);
            _linesToDraw = _solarSystems
                .Where(x => x.NeighborsByName.All(y => _solarSystemNameLookup.ContainsKey(y)))
                .SelectMany(x => x.NeighborsByName,
                    (x, y) => (x.MapLocation + _solarSystemOrigin, _solarSystemNameLookup[y].MapLocation + _solarSystemOrigin))
                .Distinct()
                .ToList();
            _solarSystemFont = Art.UIMediumFont;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Locked to screen
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.Backgrounds.BlueNebula1, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.75f);
            spriteBatch.End();

            // Locked to world
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, MainGame.Camera.Transform);
            foreach (var lineToDraw in _linesToDraw)
            {
                Art.DrawLine(spriteBatch, lineToDraw.Item1, lineToDraw.Item2, Color.Gray, 2);
            }
            foreach (var solarSystem in _solarSystems)
            {
                spriteBatch.Draw(Art.Map.SolarSystem, solarSystem.MapLocation, null, Color.Gray, 0f, _solarSystemOrigin, 0.2f, 0, 0);
                var textSize = _solarSystemFont.MeasureString(solarSystem.Name);
                var textLocation = solarSystem.MapLocation - new Vector2(textSize.X / 2, textSize.Y + 22) + _solarSystemOrigin;
                spriteBatch.DrawString(_solarSystemFont, solarSystem.Name, textLocation, Color.White);
            }
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
