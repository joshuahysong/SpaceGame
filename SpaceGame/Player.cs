using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Contracts;
using System;

namespace SpaceGame
{
    public class Player : Entity, IFocusable
    {
        protected Vector2 Size => _image == null ? Vector2.Zero : new Vector2(_image.Width, _image.Height);
        public Vector2 Center => _image == null ? Vector2.Zero : new Vector2(_image.Width / 2, _image.Height / 2);

        private Vector2 _acceleration;
        private float _currentTurnRate;
        private bool _isManeuvering;

        private readonly Texture2D _image;
        private readonly float _thrust;
        private readonly float _maneuveringThrust;
        private readonly float _maxTurnRate;
        private readonly float _maxVelocity;
        private readonly float _imageRotationOverride;

        public Player(Vector2 spawnPosition, float spawnHeading)
        {
            Position = spawnPosition;
            _image = Art.TestShip;
            _heading = spawnHeading;
            _thrust = 300f;
            _maxTurnRate = 1f;
            _maneuveringThrust = 0.05f;
            _maxVelocity = 500f;
            _imageRotationOverride = MathHelper.ToRadians(90);
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
            spriteBatch.Draw(_image, Position, null, _color, _heading + _imageRotationOverride, Size / 2f, 1f, SpriteEffects.None, 0);
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
    }
}
