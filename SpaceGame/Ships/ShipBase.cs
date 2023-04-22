using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Contracts;
using SpaceGame.Weapons;
using System;
using System.Collections.Generic;

namespace SpaceGame.Ships
{
    public class ShipBase : Entity, IFocusable
    {
        protected Texture2D _image;
        protected float _thrust;
        protected float _maneuveringThrust;
        protected float _maxTurnRate;
        protected float _maxVelocity;
        protected float _imageRotationOverride;
        protected List<WeaponBase> _weapons;

        private Vector2 _acceleration;
        private float _currentTurnRate;
        private bool _isManeuvering;

        private Vector2 _size => _image == null ? Vector2.Zero : new Vector2(_image.Width, _image.Height);

        public ShipBase(Vector2 spawnPosition, float spawnHeading)
        {
            Position = spawnPosition;
            _heading = spawnHeading;
            _weapons = new List<WeaponBase>();
        }

        public override void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            HandleInput(deltaTime);

            // Continue rotation until turn rate reaches zero to simulate slowing
            if (_currentTurnRate > 0)
            {
                RotateClockwise(deltaTime);
            }
            else if (_currentTurnRate < 0)
            {
                RotateCounterClockwise(deltaTime);
            }

            _velocity += _acceleration * deltaTime;

            // Cap velocity to max velocity
            if (_velocity.LengthSquared() > _maxVelocity * _maxVelocity)
            {
                _velocity.Normalize();
                _velocity *= _maxVelocity;
            }
            _acceleration.X = 0;
            _acceleration.Y = 0;
            Position += _velocity * deltaTime;

            if (!_isManeuvering)
            {
                if (_currentTurnRate != 0)
                {
                    SlowDownManueveringThrust();
                }
            }
            _isManeuvering = false;

            if (MainGame.IsDebugging)
            {
                MainGame.Instance.PlayerDebugEntries["Position"] = $"{Math.Round(Position.X)}, {Math.Round(Position.Y)}";
                MainGame.Instance.PlayerDebugEntries["Velocity"] = $"{Math.Round(_velocity.X)}, {Math.Round(_velocity.Y)}";
                MainGame.Instance.PlayerDebugEntries["Heading"] = $"{Math.Round(_heading, 2)}";
                MainGame.Instance.PlayerDebugEntries["Velocity Heading"] = $"{Math.Round(_velocity.ToAngle(), 2)}";
                MainGame.Instance.PlayerDebugEntries["World Tile"] = $"{Math.Floor(Position.X / MainGame.WorldTileSize)}, {Math.Floor(Position.Y / MainGame.WorldTileSize)}";
                MainGame.Instance.PlayerDebugEntries["Current Turn Rate"] = $"{Math.Round(_currentTurnRate, 2)}";
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(_image, Position, null, _color, _heading + _imageRotationOverride, _size / 2f, 1f, SpriteEffects.None, 0);
        }

        private void HandleInput(float deltaTime)
        {
            if (MainGame.Camera.Focus == this)
            {
                if (Input.IsKeyPressed(Keys.W) || Input.IsKeyPressed(Keys.Up))
                {
                    ApplyForwardThrust();
                }
                if (Input.IsKeyPressed(Keys.A) || Input.IsKeyPressed(Keys.Left))
                {
                    ApplyStarboardManeuveringThrusters();
                }
                else if (Input.IsKeyPressed(Keys.D) || Input.IsKeyPressed(Keys.Right))
                {
                    ApplyPortManeuveringThrusters();
                }
                else if (Input.IsKeyPressed(Keys.S) || Input.IsKeyPressed(Keys.Down) || Input.IsKeyPressed(Keys.X))
                {
                    RotateToRetro(deltaTime, Input.IsKeyPressed(Keys.X));
                }

                if (Input.IsKeyPressed(Keys.Space))
                {
                    FireWeapons();
                }
            }
        }

        private void ApplyForwardThrust()
        {
            _acceleration.X += _thrust * (float)Math.Cos(_heading);
            _acceleration.Y += _thrust * (float)Math.Sin(_heading);
        }

        private void ApplyPortManeuveringThrusters()
        {
            _currentTurnRate = _currentTurnRate + _maneuveringThrust > _maxTurnRate
                ? _maxTurnRate
                : _currentTurnRate + _maneuveringThrust;
            _isManeuvering = true;
        }

        private void ApplyStarboardManeuveringThrusters()
        {
            _currentTurnRate = _currentTurnRate - _maneuveringThrust < -_maxTurnRate
                ? -_maxTurnRate
                : _currentTurnRate - _maneuveringThrust;
            _isManeuvering = true;
        }

        private void RotateClockwise(float deltaTime)
        {
            _heading += _currentTurnRate * deltaTime;
            if (_heading > Math.PI)
            {
                _heading = (float)-Math.PI;
            }
        }

        private void RotateCounterClockwise(float deltaTime)
        {
            _heading += _currentTurnRate * deltaTime;
            if (_heading < -Math.PI)
            {
                _heading = (float)Math.PI;
            }
        }

        private void RotateToRetro(float deltaTime, bool IsBraking)
        {
            _isManeuvering = true;
            float movementHeading = _velocity.ToAngle();
            float retroHeading = movementHeading < 0 ? movementHeading + (float)Math.PI : movementHeading - (float)Math.PI;
            if (_heading != retroHeading && !IsWithinBrakingRange())
            {
                double retroDegrees = (retroHeading + Math.PI) * (180.0 / Math.PI);
                double headingDegrees = (_heading + Math.PI) * (180.0 / Math.PI);
                double turnRateDegrees = Math.PI * 2 * (_currentTurnRate * deltaTime) / 100 * 360 * 2;
                turnRateDegrees = turnRateDegrees < 0 ? turnRateDegrees * -1 : turnRateDegrees;
                double retroOffset = headingDegrees < retroDegrees ? (headingDegrees + 360) - retroDegrees : headingDegrees - retroDegrees;

                double thrustMagnitude = Math.Round(_currentTurnRate / _maneuveringThrust * _currentTurnRate);
                thrustMagnitude = thrustMagnitude < 0 ? thrustMagnitude * -1 : thrustMagnitude;

                if (retroOffset >= 360 - turnRateDegrees || retroOffset <= turnRateDegrees)
                {
                    _heading = retroHeading;
                    _currentTurnRate = 0;
                }
                else if (retroOffset > thrustMagnitude && 360 - retroOffset > thrustMagnitude)
                {
                    if (retroOffset < 180)
                    {
                        _currentTurnRate = _currentTurnRate - _maneuveringThrust < -_maxTurnRate ? -_maxTurnRate
                            : _currentTurnRate - _maneuveringThrust;
                    }
                    else
                    {
                        _currentTurnRate = _currentTurnRate + _maneuveringThrust > _maxTurnRate ? _maxTurnRate
                            : _currentTurnRate + _maneuveringThrust;
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
                    _velocity = Vector2.Zero;
                }
                else if (_heading == retroHeading)
                {
                    _acceleration.X += _thrust * (float)Math.Cos(_heading);
                    _acceleration.Y += _thrust * (float)Math.Sin(_heading);
                }
            }
        }

        private void SlowDownManueveringThrust()
        {
            if (_currentTurnRate < 0)
            {
                _currentTurnRate = _currentTurnRate + _maneuveringThrust > 0 ? 0 : _currentTurnRate + _maneuveringThrust;
            }
            else if (_currentTurnRate > 0)
            {
                _currentTurnRate = _currentTurnRate - _maneuveringThrust < 0 ? 0 : _currentTurnRate - _maneuveringThrust;
            }
        }

        private bool IsWithinBrakingRange()
        {
            double brakingRange = _maxVelocity / 100;
            return _velocity.X < brakingRange && _velocity.X > -brakingRange && _velocity.Y < brakingRange && _velocity.Y > -brakingRange;
        }

        private void FireWeapons()
        {
            _weapons.ForEach(weapon => weapon.Fire(_heading, _velocity, Position));
        }
    }
}
