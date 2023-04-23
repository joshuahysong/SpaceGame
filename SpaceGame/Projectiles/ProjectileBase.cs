using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Projectiles
{
    public class ProjectileBase : IEntity
    {
        public Vector2 Position { get; set; }
        public Vector2 TileCoordinates { get; set; }
        public bool IsExpired { get; set; }

        public Vector2 _previousPosition;
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
            Position = position;
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

            _previousPosition = Position;
            Position += _velocity * deltaTime;

            _timeAlive += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeAlive > _timeToLiveInSeconds)
            {
                IsExpired = true;
            }
            CheckForCollision();
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(_image, Position, null, _color, _heading, _size / 2f, _scale, 0, 0);

            if (MainGame.IsDebugging && _previousPosition != Vector2.Zero)
            {
                Art.DrawLine(spriteBatch, _previousPosition, Position, Color.Red);
            }
        }

        private void CheckForCollision()
        {
            var coordinatesToCheck = new List<Vector2>
            {
                TileCoordinates,
                new Vector2(TileCoordinates.X - 1, TileCoordinates.Y),
                new Vector2(TileCoordinates.X - 1, TileCoordinates.Y - 1),
                new Vector2(TileCoordinates.X - 1, TileCoordinates.Y + 1),
                new Vector2(TileCoordinates.X - 1, TileCoordinates.Y),
                new Vector2(TileCoordinates.X + 1, TileCoordinates.Y - 1),
                new Vector2(TileCoordinates.X + 1, TileCoordinates.Y + 1),
                new Vector2(TileCoordinates.X, TileCoordinates.Y - 1),
                new Vector2(TileCoordinates.X, TileCoordinates.Y + 1)
            };
            var validEntities = EntityManager.Entities
                .Where(x => x.TileCoordinates == TileCoordinates
                    && x != this).ToList();
            if (validEntities.Count > 0)
            {

            }
        }
    }
}
