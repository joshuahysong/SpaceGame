using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.SolarSystems.Models;
using System.Collections.Generic;

namespace SpaceGame.SolarSystems
{
    public class TestSystem1 : ISolarSystem
    {
        public string Name { get; set; }
        public FactionType Faction { get; set; }
        public List<Planet> Planets { get; set; }

        public TestSystem1()
        {
            Planets = new List<Planet>
            {
                new Planet(FactionType.None, Vector2.Zero, Art.Planets.RedPlanet)
            };
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var planet in Planets)
            {
                planet.Draw(spriteBatch, Matrix.Identity);
            }

        }
    }
}
