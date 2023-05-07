using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Common;
using SpaceGame.Managers;
using System;

namespace SpaceGame.Projectiles
{
    public abstract class ProjectileBase : IEntity, ICollidable
    {
        public Guid Id { get; set; }
        public FactionType Faction { get; set; }
        public Vector2 Position { get; set; }
        public bool IsExpired { get; set; }
        public Texture2D Texture { get; set; }
        public Color[] TextureData { get; set; }
        public float Scale { get; set; }

        public Matrix Transform => Matrix.CreateTranslation(new Vector3(-_origin, 0.0f) * Scale)
            * Matrix.CreateRotationZ(_heading)
            * Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        public Rectangle BoundingRectangle => CollisionManager.CalculateBoundingRectangle(_rectangle, Transform);

        protected Vector2 _velocity;

        private readonly Rectangle _rectangle;
        private readonly Vector2 _origin;
        private Texture2D _boundingBoxTexture;
        private Vector2 _previousPosition;
        private float _heading;
        private long _timeToLiveInSeconds;
        private double _timeAlive;

        public ProjectileBase(
            FactionType faction,
            Vector2 position,
            Vector2 velocity,
            float heading,
            Texture2D texture,
            long timeToLiveInSeconds,
            float scale = 1f)
        {
            Id = Guid.NewGuid();
            Faction = faction;
            Position = position;
            Texture = texture;
            Scale = scale;
            TextureData = Art.GetScaledTextureData(Texture, Scale);
            _velocity = velocity;
            _timeToLiveInSeconds = timeToLiveInSeconds;
            _heading = heading;
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, (int)Math.Floor(Texture.Width * Scale), (int)Math.Floor(Texture.Height * Scale));
            _boundingBoxTexture = Art.CreateRectangleTexture(BoundingRectangle.Width, BoundingRectangle.Height, Color.Transparent, Color.White);
            CollisionManager.Add(this);
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _previousPosition = Position;
            Position += _velocity * deltaTime;

            _timeAlive += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeAlive > _timeToLiveInSeconds)
            {
                IsExpired = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform, bool drawMinimized = false)
        {
            if (drawMinimized) return;

            spriteBatch.Draw(Texture, Position, null, Color.White, _heading, _origin, Scale, 0, 0);

            if (MainGame.IsDebugging && _previousPosition != Vector2.Zero)
            {
                Art.DrawLine(spriteBatch, _previousPosition, Position, Color.Red);
                spriteBatch.Draw(_boundingBoxTexture, BoundingRectangle, Color.White);
            }
        }

        public abstract float PerformHitEffect();
    }
}
