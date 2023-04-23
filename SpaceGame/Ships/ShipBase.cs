using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Weapons;
using System;
using System.Collections.Generic;

namespace SpaceGame.Ships
{
    public class ShipBase
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Heading;
        public float CurrentTurnRate;
        public bool IsManeuvering;

        protected Texture2D _image;
        protected float _scale;
        protected float _thrust;
        protected float _maneuveringThrust;
        protected float _maxTurnRate;
        protected float _maxVelocity;
        protected float _imageRotationOverride;
        protected List<WeaponBase> _weapons;

        private Vector2 _acceleration;

        private Vector2 _size => _image == null ? Vector2.Zero : new Vector2(_image.Width, _image.Height);

        public ShipBase(
            Vector2 spawnPosition,
            float spawnHeading,
            Texture2D image,
            float thrust,
            float maneuveringThrust,
            float maxTurnRate,
            float maxVelocity,
            float scale = 1f,
            float imageRotationOverride = 0f)
        {
            Position = spawnPosition;
            Heading = spawnHeading;
            _image = image;
            _thrust = thrust;
            _maneuveringThrust = maneuveringThrust;
            _maxTurnRate = maxTurnRate;
            _maxVelocity = maxVelocity;
            _scale = scale;
            _imageRotationOverride = imageRotationOverride;
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
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(_image, Position, null, Color.White, Heading + _imageRotationOverride, _size / 2f, _scale, SpriteEffects.None, 0);

            if (MainGame.IsDebugging)
            {
                Art.DrawLine(spriteBatch, Position, Position + Velocity, Color.Blue);
                Art.DrawLine(spriteBatch, Position, _maxVelocity, Heading, Color.Green);
                var scaledSize = _size * _scale;
                var radius = Math.Sqrt(Math.Pow(scaledSize.X, 2) + Math.Pow(scaledSize.Y, 2)) / 2;
                Art.DrawCircle(spriteBatch, Position, (int)Math.Ceiling(radius), Color.Yellow * 0.75f);
                Art.DrawRectangle(spriteBatch, Position, scaledSize, Heading + _imageRotationOverride, Color.Turquoise);
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
