using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Projectiles
{
    public class ProjectileBase : Entity
    {
        protected Texture2D _image;
        protected long _timeToLiveInSeconds;

        private double _timeAlive;

        private Vector2 _size => _image == null ? Vector2.Zero : new Vector2(_image.Width, _image.Height);

        public ProjectileBase(Vector2 position, Vector2 velocity)
        {
            Position = position;
            _velocity = velocity;
            _heading = _velocity.ToAngle();
        }

        public override void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_velocity.LengthSquared() > 0)
            {
                _heading = _velocity.ToAngle();
            }

            Position += _velocity * deltaTime;

            _timeAlive += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeAlive > _timeToLiveInSeconds)
            {
                IsExpired = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            // TODO Only draw if on screen
            spriteBatch.Draw(_image, Position, null, _color, _heading, _size / 2f, 1f, 0, 0);
        }
    }
}
