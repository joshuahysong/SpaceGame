using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Common
{
    public interface IEntity
    {
        public Vector2 Position { get; }
        public bool IsExpired { get; }
        public string CurrentSolarSystemName { get; set; }

        public abstract void Update(GameTime gameTime, Matrix parentTransform);

        public abstract void Draw(SpriteBatch spriteBatch, Matrix parentTransform, bool drawMinimized);
    }
}
