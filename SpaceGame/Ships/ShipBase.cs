using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
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

        public Texture2D Texture { get; set; }
        public Color[] TextureData { get; set; }
        public float Scale { get; set; }

        public Matrix Transform => Matrix.CreateTranslation(new Vector3(-_origin, 0.0f) * Scale)
            * Matrix.CreateRotationZ(Heading)
            * Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        public Rectangle BoundingRectangle => CollisionManager.CalculateBoundingRectangle(_rectangle, Transform);

        protected float _thrust;
        protected float _maneuveringThrust;
        protected float _maxTurnRate;
        protected float _maxVelocity;
        protected List<WeaponBase> _weapons;

        private Vector2 _acceleration;
        private bool _hasCollision;
        private Rectangle _rectangle;
        private Vector2 _origin;

        public ShipBase(
            Vector2 spawnPosition,
            float spawnHeading,
            Texture2D texture,
            float thrust,
            float maneuveringThrust,
            float maxTurnRate,
            float maxVelocity,
            float scale = Constants.Scales.Full)
        {
            Position = spawnPosition;
            Heading = spawnHeading;
            Texture = texture;
            Scale = scale;
            _thrust = thrust;
            _maneuveringThrust = maneuveringThrust;
            _maxTurnRate = maxTurnRate;
            _maxVelocity = maxVelocity;
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, (int)Math.Floor(Texture.Width * Scale), (int)Math.Floor(Texture.Height * Scale));
            TextureData = Art.GetScaledTextureData(Texture, Scale);
            CollisionManager.Add(this);
            _weapons = new List<WeaponBase>();
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
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Heading, _origin, Scale, SpriteEffects.None, 0);

            if (MainGame.IsDebugging)
            {
                Art.DrawLine(spriteBatch, Position, Position + Velocity, Color.Blue);
                Art.DrawLine(spriteBatch, Position, _maxVelocity, Heading, Color.Green);

                var color = _hasCollision ? Color.Red : Color.White;
                var boundingTexture = Art.CreateRectangle(BoundingRectangle.Width, BoundingRectangle.Height, Color.Transparent, color);
                spriteBatch.Draw(boundingTexture, BoundingRectangle, Color.White);
            }
        }

        public void FireWeapons()
        {
            _weapons.ForEach(weapon => weapon.Fire(Heading, Velocity, Position));
        }

        public void ApplyForwardThrust()
        {
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
