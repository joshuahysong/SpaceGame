using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.SolarSystems.Models;
using System.Collections.Generic;

namespace SpaceGame.SolarSystems
{
    public class TestSystem2 : ISolarSystem
    {
        public string Name { get; }
        public FactionType Faction { get; }
        public Vector2 MapLocation { get; }
        public List<string> NeighborsByName { get; }
        public List<Planet> Planets { get; }

        public TestSystem2()
        {
            Name = "Test System 2";
            Faction = FactionType.None;
            MapLocation = new Vector2(-50, -50);
            NeighborsByName = new List<string> { "Test System 1" };
            Planets = new List<Planet> { CreatePlanet1() };
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var planet in Planets)
            {
                planet.Draw(spriteBatch, Matrix.Identity);
            }
        }

        public void DrawMini(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var planet in Planets)
            {
                planet.DrawMini(spriteBatch, Matrix.Identity);
            }
        }

        private Planet CreatePlanet1()
        {
            var description = new List<string>();
            description.Add($"Some interesting description here...");
            return new Planet(FactionType.None, Vector2.Zero, Art.Planets.Cloudy4, description);
        }
    }
}
