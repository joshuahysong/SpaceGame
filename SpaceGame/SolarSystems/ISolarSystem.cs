using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.SolarSystems.Models;
using System.Collections.Generic;

namespace SpaceGame.SolarSystems
{
    public interface ISolarSystem
    {
        public string Name { get; }
        public FactionType Faction { get; }
        public Vector2 MapLocation { get; }
        public List<string> NeighborsByName { get; }
        public List<Planet> Planets { get; }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public void DrawMini(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
