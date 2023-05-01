using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
using SpaceGame.Ships;

namespace SpaceGame.Entities
{
    public class Dummy : IEntity
    {
        public Vector2 Position => Ship?.Position ?? Vector2.Zero;
        public bool IsExpired => Ship?.IsExpired ?? false;

        public ShipBase Ship { get; set; }

        public Dummy(ShipBase ship)
        {
            Ship = ship;
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            Ship.Update(gameTime, parentTransform);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            Ship.Draw(spriteBatch, parentTransform);
        }
    }
}
