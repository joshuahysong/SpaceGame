using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpaceGame.SolarSystems.Models;
using System.Collections.Generic;

namespace SpaceGame.SolarSystems
{
    public interface ISolarSystem
    {
        public string Name { get; set; }
        public FactionType Faction { get; set; }
        public List<Planet> Planets { get; set; }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
