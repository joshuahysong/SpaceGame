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
            Planets = planets;
            Background = background;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, bool drawMinimized = false)
        {
            foreach (var planet in Planets)
            {
                planet.Draw(spriteBatch, Matrix.Identity, drawMinimized);
            }
        }
    }
}
