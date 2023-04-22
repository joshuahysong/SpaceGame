using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame
{
    public abstract class Entity
    {
        public Vector2 Position { get; set; }
        public bool IsExpired { get; set; }

        protected float _heading;
        protected Vector2 _velocity;
        protected Color _color = Color.White;

        public abstract void Update(GameTime gameTime, Matrix parentTransform);

        public abstract void Draw(SpriteBatch spriteBatch, Matrix parentTransform);
    }
}
