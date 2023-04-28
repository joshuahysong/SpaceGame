using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Timers;
using SpaceGame.Managers;
using SpaceGame.ParticleEffects;
using SpaceGame.Projectiles;
using SpaceGame.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Ships
{
    public class ShipBase : ICollidable
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Heading;
        public float CurrentTurnRate;
        public bool IsManeuvering;

        public FactionType Faction { get; set; }
        public Texture2D Texture { get; set; }
        public Color[] TextureData { get; set; }
        public float Scale { get; set; }
        public bool IsExpired { get; set; }

        public Matrix Transform => Matrix.CreateTranslation(new Vector3(-_origin, 0.0f) * Scale)
            * Matrix.CreateRotationZ(Heading)
            * Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        public Rectangle BoundingRectangle => CollisionManager.CalculateBoundingRectangle(_rectangle, Transform);

        protected float _thrust;
        protected float _maneuveringThrust;
        protected float _maxTurnRate;
        protected float _maxVelocity;
        protected Vector2 _acceleration;
        protected bool _hasCollision;
        protected Rectangle _rectangle;
        protected Vector2 _origin;
        protected Texture2D _boundingBoxTexture;
        protected int _maxHealth;
        protected int _currentHealth;

        protected List<WeaponBase> _weapons = new();
        protected List<IParticleEffect> _thrustEffects = new();

        private bool _isThrusting;

        public ShipBase(
            FactionType faction,
            Vector2 spawnPosition,
            float spawnHeading,
            Texture2D texture,
            float thrust,
            float maneuveringThrust,
            float maxTurnRate,
            float maxVelocity,
            int maxHealth,
            float scale = ScaleType.Full)
        {
            Faction = faction;
            Position = spawnPosition;
            Heading = spawnHeading;
            Texture = texture;
            Scale = scale;
            _thrust = thrust;
            _maneuveringThrust = maneuveringThrust;
            _maxTurnRate = maxTurnRate;
            _maxVelocity = maxVelocity;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, (int)Math.Floor(Texture.Width * Scale), (int)Math.Floor(Texture.Height * Scale));
            TextureData = Art.GetScaledTextureData(Texture, Scale);
            _boundingBoxTexture = Art.CreateRectangle(BoundingRectangle.Width, BoundingRectangle.Height, Color.Transparent, Color.White);
            CollisionManager.Add(this);
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Continue rotation until turn rate reaches zero to simulate slowing
            if (CurrentTurnRate > 0)
            {
                RotateClockwise(deltaTime);
            }
            else if (CurrentTurnRate < 0)
            {
                RotateCounterClockwise(deltaTime);
            }

            Velocity += _acceleration * deltaTime;

            // Cap velocity to max velocity
            if (Velocity.LengthSquared() > _maxVelocity * _maxVelocity)
            {
                Velocity.Normalize();
                Velocity *= _maxVelocity;
            }
            _acceleration.X = 0;
            _acceleration.Y = 0;
            Position += Velocity * deltaTime;

            if (!IsManeuvering)
            {
                if (CurrentTurnRate != 0)
                {
                    SlowDownManueveringThrust();
                }
            }
            IsManeuvering = false;

            _hasCollision = false;
            var collisions = CollisionManager.GetCollisions(this);
            if (collisions.Any())
            {
                _hasCollision = true;
                foreach (var collision in collisions.Where(x => x is ProjectileBase))
                {
                    var projectile = (ProjectileBase)collision;
                    _currentHealth -= projectile.PerformHitEffect();
                    projectile.IsExpired = true;
                }
            }

            if (_thrustEffects.Any())
            {
                foreach (var thrustEffect in _thrustEffects)
                {
                    thrustEffect.ParticleEffect.Emitters.First().AutoTrigger = _isThrusting;
                    thrustEffect.Rotation = Velocity;
                    thrustEffect.Position = Position;
                    thrustEffect.Update(gameTime);
                }
            }

            if (_currentHealth <= 0)
            {
                IsExpired = true;
            }
            _isThrusting = false;
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Heading, _origin, Scale, SpriteEffects.None, 0);

            if (_thrustEffects.Any())
            {
                foreach (var thrustEffect in _thrustEffects)
                {
                    thrustEffect.Draw(spriteBatch);
                }
            }

            if (MainGame.IsDebugging)
            {
                Art.DrawLine(spriteBatch, Position, Position + Velocity, Color.Blue);
                Art.DrawLine(spriteBatch, Position, _maxVelocity, Heading, Color.Green);
                spriteBatch.Draw(_boundingBoxTexture, BoundingRectangle, _hasCollision ? Color.Red : Color.White);
            }
        }

        public void FireWeapons()
        {
            _weapons.ForEach(weapon => weapon.Fire(Faction, Heading, Velocity, Position));
        }

        public void ApplyForwardThrust()
        {
            _isThrusting = true;
            _acceleration.X += _thrust * (float)Math.Cos(Heading);
            _acceleration.Y += _thrust * (float)Math.Sin(Heading);
        }

        public void ApplyPortManeuveringThrusters()
        {
            CurrentTurnRate = CurrentTurnRate + _maneuveringThrust > _maxTurnRate
                ? _maxTurnRate
                : CurrentTurnRate + _maneuveringThrust;
            IsManeuvering = true;
        }

        public void ApplyStarboardManeuveringThrusters()
        {
            CurrentTurnRate = CurrentTurnRate - _maneuveringThrust < -_maxTurnRate
                ? -_maxTurnRate
                : CurrentTurnRate - _maneuveringThrust;
            IsManeuvering = true;
        }

        public void RotateToRetro(float deltaTime, bool IsBraking)
        {
            IsManeuvering = true;
            float movementHeading = Velocity.ToAngle();
            float retroHeading = movementHeading < 0 ? movementHeading + (float)Math.PI : movementHeading - (float)Math.PI;
            if (Heading != retroHeading && !IsWithinBrakingRange())
            {
                double retroDegrees = (retroHeading + Math.PI) * (180.0 / Math.PI);
                double headingDegrees = (Heading + Math.PI) * (180.0 / Math.PI);
                double turnRateDegrees = Math.PI * 2 * (CurrentTurnRate * deltaTime) / 100 * 360 * 2;
                turnRateDegrees = turnRateDegrees < 0 ? turnRateDegrees * -1 : turnRateDegrees;
                double retroOffset = headingDegrees < retroDegrees ? (headingDegrees + 360) - retroDegrees : headingDegrees - retroDegrees;

                double thrustMagnitude = Math.Round(CurrentTurnRate / _maneuveringThrust * CurrentTurnRate);
                thrustMagnitude = thrustMagnitude < 0 ? thrustMagnitude * -1 : thrustMagnitude;

                if (retroOffset >= 360 - turnRateDegrees || retroOffset <= turnRateDegrees)
                {
                    Heading = retroHeading;
                    CurrentTurnRate = 0;
                }
                else if (retroOffset > thrustMagnitude && 360 - retroOffset > thrustMagnitude)
                {
                    if (retroOffset < 180)
                    {
                        CurrentTurnRate = CurrentTurnRate - _maneuveringThrust < -_maxTurnRate ? -_maxTurnRate
                            : CurrentTurnRate - _maneuveringThrust;
                    }
                    else
                    {
                        CurrentTurnRate = CurrentTurnRate + _maneuveringThrust > _maxTurnRate ? _maxTurnRate
                            : CurrentTurnRate + _maneuveringThrust;
                    }
                }
                else
                {
                    SlowDownManueveringThrust();
                }
            }
            else if (IsBraking)
            {
                if (IsWithinBrakingRange())
                {
                    Velocity = Vector2.Zero;
                }
                else if (Heading == retroHeading)
                {
                    _acceleration.X += _thrust * (float)Math.Cos(Heading);
                    _acceleration.Y += _thrust * (float)Math.Sin(Heading);
                }
            }
        }

        private void RotateClockwise(float deltaTime)
        {
            Heading += CurrentTurnRate * deltaTime;
            if (Heading > Math.PI)
            {
                Heading = (float)-Math.PI;
            }
        }

        private void RotateCounterClockwise(float deltaTime)
        {
            Heading += CurrentTurnRate * deltaTime;
            if (Heading < -Math.PI)
            {
                Heading = (float)Math.PI;
            }
        }

        private void SlowDownManueveringThrust()
        {
            if (CurrentTurnRate < 0)
            {
                CurrentTurnRate = CurrentTurnRate + _maneuveringThrust > 0 ? 0 : CurrentTurnRate + _maneuveringThrust;
            }
            else if (CurrentTurnRate > 0)
            {
                CurrentTurnRate = CurrentTurnRate - _maneuveringThrust < 0 ? 0 : CurrentTurnRate - _maneuveringThrust;
            }
        }

        private bool IsWithinBrakingRange()
        {
            double brakingRange = _maxVelocity / 100;
            return Velocity.X < brakingRange && Velocity.X > -brakingRange && Velocity.Y < brakingRange && Velocity.Y > -brakingRange;
        }
    }
}
