using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SpaceGame.Scenes.Components
{
    public class SolarSystem
    {
        public string Name { get; }
        public FactionType Faction { get; }
        public Vector2 MapLocation { get; }
        public List<string> NeighborsByName { get; }
        public List<Planet> Planets { get; }
        public Texture2D Background { get; }

        public SolarSystem(string name,
            FactionType faction,
            Vector2 mapLocation,
            List<string> neighborsByName,
            List<Planet> planets,
            Texture2D background)
        {
            Name = name;
            Faction = faction;
            MapLocation = mapLocation;
            NeighborsByName = neighborsByName;
            Planets = new List<Planet> { CreatePlanet1() };
            Background = background;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, bool drawMinimized = false)
        {
            foreach (var planet in Planets)
            {
                planet.Draw(spriteBatch, Matrix.Identity, drawMinimized);
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
