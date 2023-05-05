using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpaceGame.SolarSystems.Models;
using System.Collections.Generic;

namespace SpaceGame.SolarSystems
{
    public interface ISolarSystem
    {
        public string Name { get; }
        public FactionType Faction { get; }
        public List<Planet> Planets { get; }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
