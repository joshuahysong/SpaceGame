using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Entities;

namespace SpaceGame.Projectiles
{
    public class ProjectileBase : IEntity
    {
        public bool IsExpired { get; set; }

        public Vector2 _position;
        public Vector2 _velocity;
        private float _heading;
        private Texture2D _image;
        private long _timeToLiveInSeconds;
        private double _timeAlive;
        private float _scale;
        private Color _color;

        private Vector2 _size => _image == null ? Vector2.Zero : new Vector2(_image.Width, _image.Height);

        public ProjectileBase(
            Vector2 position,
            Vector2 velocity,
            Texture2D image,
            long timeToLiveInSeconds,
            float scale = 1f,
            Color? color = null)
        {
            _position = position;
            _velocity = velocity;
            _heading = velocity.ToAngle();
            _image = image;
            _timeToLiveInSeconds = timeToLiveInSeconds;
            _scale = scale;
            _color = color ?? Color.White;
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_velocity.LengthSquared() > 0)
            {
                _heading = _velocity.ToAngle();
            }

            _position += _velocity * deltaTime;

            _timeAlive += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeAlive > _timeToLiveInSeconds)
            {
                IsExpired = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(_image, _position, null, _color, _heading, _size / 2f, _scale, 0, 0);
        }
    }
}
