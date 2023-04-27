using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using SpaceGame.Managers;
using System;
using System.Collections.Generic;

namespace SpaceGame.Projectiles
{
    public abstract class ProjectileBase : IEntity, ICollidable
    {
        public FactionType Faction { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 TileCoordinates { get; set; }
        public bool IsExpired { get; set; }
        public Texture2D Texture { get; set; }
        public Color[] TextureData { get; set; }
        public float Scale { get; set; }

        public Matrix Transform => Matrix.CreateTranslation(new Vector3(-_origin, 0.0f))
            * Matrix.CreateRotationZ(_heading)
            * Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        public Rectangle BoundingRectangle => CollisionManager.CalculateBoundingRectangle(_rectangle, Transform);

        private readonly Rectangle _rectangle;
        private readonly Vector2 _origin;
        private Vector2 _previousPosition;
        private Vector2 _velocity;
        private float _heading;
        private long _timeToLiveInSeconds;
        private double _timeAlive;

        private ParticleEffect _particleEffect;

        public ProjectileBase(
            FactionType faction,
            Vector2 position,
            Vector2 velocity,
            Texture2D texture,
            long timeToLiveInSeconds,
            float scale = 1f)
        {
            Faction = faction;
            Position = position;
            Texture = texture;
            Scale = scale;
            _velocity = velocity;
            _timeToLiveInSeconds = timeToLiveInSeconds;
            _heading = velocity.ToAngle();
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            TextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
            CollisionManager.Add(this);

            TextureRegion2D textureRegion = new TextureRegion2D(Art.Pixel);
            _particleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = new Vector2(400, 240),
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(2.5),
                        Profile.BoxUniform(100,250))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 3,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 4.0f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = new HslColor(0.33f, 0.5f, 0.5f),
                                        EndValue = new HslColor(0.5f, 0.9f, 1.0f)
                                    }
                                }
                            },
                            new RotationModifier {RotationRate = -2.1f},
                            new RectangleContainerModifier {Width = 800, Height = 480},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f},
                        }
                    }
                }
            };
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

            _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, _heading, _origin, 1f, 0, 0);
            spriteBatch.Draw(_particleEffect);

            if (MainGame.IsDebugging && _previousPosition != Vector2.Zero)
            {
                Art.DrawLine(spriteBatch, _previousPosition, Position, Color.Red);
            }
        }

        public abstract int PerformHitEffect();
    }
}
