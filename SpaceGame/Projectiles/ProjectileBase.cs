using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Projectiles
{
    public class ProjectileBase : IEntity, ICollidable
    {
        public Vector2 Position { get; set; }
        public Vector2 TileCoordinates { get; set; }
        public bool IsExpired { get; set; }

        public Texture2D Texture { get; set; }
        public Color[] TextureData { get; set; }

        public Matrix Transform => Matrix.CreateTranslation(new Vector3(-_origin, 0.0f))
            * Matrix.CreateRotationZ(_heading)
            * Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        public Rectangle BoundingRectangle => CollisionManager.CalculateBoundingRectangle(_rectangle, Transform);

        private readonly Rectangle _rectangle;
        private readonly Vector2 _origin;
        public Vector2 _previousPosition;
        public Vector2 _velocity;
        private float _heading;
        private long _timeToLiveInSeconds;
        private double _timeAlive;

        public ProjectileBase(
            Vector2 position,
            Vector2 velocity,
            Texture2D texture,
            long timeToLiveInSeconds)
        {
            Position = position;
            Texture = texture;
            _velocity = velocity;
            _timeToLiveInSeconds = timeToLiveInSeconds;
            _heading = velocity.ToAngle();
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            TextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
            CollisionManager.Add(this);
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
            spriteBatch.Draw(Texture, Position, null, Color.White, _heading, _origin, 1f, 0, 0);

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
