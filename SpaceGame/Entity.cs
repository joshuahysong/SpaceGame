using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame
{
    public abstract class Entity
    {
        public virtual Vector2 Position { get; set; }
        public bool IsExpired { get; set; }
        public float Heading { get; set; }

        protected Vector2 _velocity;
        protected Color _color = Color.White;

        public abstract void Update(GameTime gameTime, Matrix parentTransform);

        public abstract void Draw(SpriteBatch spriteBatch, Matrix parentTransform);
    }
}
