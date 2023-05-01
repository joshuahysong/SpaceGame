using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Managers
{
    public interface IEntity
    {
        public Vector2 Position { get; }
        public bool IsExpired { get; }

        public abstract void Update(GameTime gameTime, Matrix parentTransform);

        public abstract void Draw(SpriteBatch spriteBatch, Matrix parentTransform);
    }
}
