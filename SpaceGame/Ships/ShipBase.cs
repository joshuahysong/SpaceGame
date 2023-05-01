using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
using SpaceGame.Projectiles;
using SpaceGame.Ships.Parts;
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

        public Matrix LocalTransform =>
            Matrix.CreateTranslation(0, 0, 0f) *
            Matrix.CreateScale(1f, 1f, 1f) *
            Matrix.CreateRotationZ(Heading) *
            Matrix.CreateTranslation(Position.X, Position.Y, 0f);

        public Rectangle BoundingRectangle => CollisionManager.CalculateBoundingRectangle(_rectangle, Transform);

        protected List<WeaponBase> _weapons = new();
        protected List<Thruster1> _thrusters = new();

        private float _thrust;
        private float _maneuveringThrust;
        private float _maxTurnRate;
        private float _maxVelocity;
        private Vector2 _acceleration;
        private bool _hasCollision;
        private Rectangle _rectangle;
        private Vector2 _origin;
        private Texture2D _boundingBoxTexture;
        private bool _isThrusting;
        private float _maxHealth;
        private float _currentHealth;
        private float _healthBarOffset;
        private float _maxShield;
        private float _currentShield;
        private float _shieldRegen;
        private bool _showHealthBars;

        public ShipBase(
            FactionType faction,
            Vector2 spawnPosition,
            float spawnHeading,
            Texture2D texture,
            float thrust,
            float maneuveringThrust,
            float maxTurnRate,
            float maxVelocity,
            float maxHealth,
            float maxShield,
            float shieldRegen,
            float scale = ScaleType.Full,
            bool showHealthBars = true)
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
            _maxShield = maxShield;
            _currentShield = maxShield;
            _shieldRegen = shieldRegen;
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, (int)Math.Floor(Texture.Width * Scale), (int)Math.Floor(Texture.Height * Scale));
            TextureData = Art.GetScaledTextureData(Texture, Scale);
            _boundingBoxTexture = Art.CreateRectangleTexture(BoundingRectangle.Width, BoundingRectangle.Height, Color.Transparent, Color.White);
            _healthBarOffset = (Texture.Width > Texture.Height ? Texture.Width : Texture.Height) / 2 * Scale + 10;
            _showHealthBars = showHealthBars;

            CollisionManager.Add(this);
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Matrix globalTransform = LocalTransform * parentTransform;

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

            foreach (var weapon in _weapons)
            {
                weapon.Update(gameTime);
            }

            _hasCollision = false;
            var collisions = CollisionManager.GetCollisions(this);
            if (collisions.Any())
            {
                _hasCollision = true;
                foreach (var collision in collisions.Where(x => x is ProjectileBase))
                {
                    var projectile = (ProjectileBase)collision;
                    var damage = projectile.PerformHitEffect();
                    if (_currentShield > 0)
                    {
                        if (_currentShield < damage)
                        {
                            damage -= _currentShield;
                            _currentShield = 0;
                        }
                        _currentShield -= damage;
                    }
                    else
                    {
                        _currentHealth -= damage;
                    }
                    projectile.IsExpired = true;
                }
            }

            if (_currentShield < _maxShield)
            {
                _currentShield += _shieldRegen * deltaTime;
                _currentShield = _currentShield > _maxShield ? _maxShield : _currentShield;
            }

            if (_currentHealth <= 0)
            {
                IsExpired = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            Matrix globalTransform = LocalTransform * parentTransform;

            if (_thrusters.Any() && _isThrusting)
            {
                foreach (var thrustEffect in _thrusters)
                {
                    thrustEffect.Draw(spriteBatch, globalTransform);
                }
                _isThrusting = false;
            }

            spriteBatch.Draw(Texture, Position, null, Color.White, Heading, _origin, Scale, SpriteEffects.None, 0);

            if (_showHealthBars && _currentHealth < _maxHealth || _currentShield < _maxShield)
            {
                var healthBarLength = 60f * _currentHealth / _maxHealth;
                Art.DrawLine(spriteBatch, Position + new Vector2(-30, _healthBarOffset), Position + new Vector2(-30 + healthBarLength, _healthBarOffset), Color.Red, 2);

                var shieldBarLength = 60f * _currentShield / _maxShield;
                Art.DrawLine(spriteBatch, Position + new Vector2(-30, _healthBarOffset + 5), Position + new Vector2(-30 + shieldBarLength, _healthBarOffset + 5), Color.Turquoise, 2);
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
            _acceleration.X += _thrust * (float)Math.Cos(Heading);
            _acceleration.Y += _thrust * (float)Math.Sin(Heading);
            _isThrusting = true;
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
                    _isThrusting = true;
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
