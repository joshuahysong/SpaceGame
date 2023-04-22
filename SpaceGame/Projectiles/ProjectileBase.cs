using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Projectiles
{
    public class ProjectileBase : Entity
    {
        private Texture2D _image;
        private long _timeToLiveInSeconds;
        private double _timeAlive;
        private float _scale;

        private Vector2 _size => _image == null ? Vector2.Zero : new Vector2(_image.Width, _image.Height);

        public ProjectileBase(
            Vector2 position,
            Vector2 velocity,
            Texture2D image,
            long timeToLiveInSeconds,
            float scale = 1f)
        {
            Position = position;
            _velocity = velocity;
            Heading = _velocity.ToAngle();
            _image = image;
            _timeToLiveInSeconds = timeToLiveInSeconds;
            _scale = scale;
        }

        public override void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_velocity.LengthSquared() > 0)
            {
                Heading = _velocity.ToAngle();
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
            spriteBatch.Draw(_image, Position, null, _color, Heading, _size / 2f, _scale, 0, 0);
        }
    }
}
