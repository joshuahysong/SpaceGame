using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.SolarSystems.Models;
using System.Collections.Generic;

namespace SpaceGame.SolarSystems
{
    public class TestSystem1 : ISolarSystem
    {
        public string Name { get; }
        public FactionType Faction { get; }
        public Vector2 MapLocation { get; }
        public List<string> NeighborsByName { get; }
        public List<Planet> Planets { get; }

        public TestSystem1()
        {
            Name = "Test System 1";
            Faction = FactionType.None;
            MapLocation = new Vector2(50, 50);
            NeighborsByName = new List<string> { "Test System 2" };
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
            description.Add($"The sun beats down on the empty spaceport, its harsh rays reflecting off the sand dunes." +
                $"The only sound is the wind whistling through the abandoned buildings." +
                $"The spaceport is a reminder of a time when this planet was thriving, but now it is nothing more than a ghost town.");
            description.Add($"The once-busy spaceport is now deserted. The docking bays are empty, the hangars are closed, and the control tower is silent." +
                $"The only signs of life are the occasional sand lizards that scuttle across the tarmac.");
            description.Add($"The spaceport is a relic of a bygone era." +
                $"It was once a hub of commerce and trade, but now it is nothing more than a monument to a lost civilization. The buildings are crumbling," +
                $"the ships are rusting, and the sand is slowly encroaching on the tarmac.");
            description.Add($"The spaceport is a reminder of the fragility of life." +
                $"It is a reminder that even the most prosperous civilizations can be brought to ruin. It is a reminder that nothing lasts forever.");
            return new Planet(FactionType.None, Vector2.Zero, Art.Planets.RedPlanet, description);
        }
    }
}
