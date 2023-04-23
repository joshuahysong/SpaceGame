using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Entities
{
    public interface IEntity
    {
        public Vector2 Position { get; }
        public Vector2 TileCoordinates { get; set; }
        public bool IsExpired { get; set; }

        public abstract void Update(GameTime gameTime, Matrix parentTransform);

        public abstract void Draw(SpriteBatch spriteBatch, Matrix parentTransform);
    }
}
