using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Entities
{
    public interface IEntity
    {
        public bool IsExpired { get; set; }

        public abstract void Update(GameTime gameTime, Matrix parentTransform);

        public abstract void Draw(SpriteBatch spriteBatch, Matrix parentTransform);
    }
}
