using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
using SpaceGame.Projectiles;
using System;

namespace SpaceGame.Weapons
{
    public class WeaponBase
    {
        protected float _cooldown;
        protected float _speed;
        protected float _accuracy;
        protected Type _projectileType;
        protected Vector2 _position;
        protected Vector2 _projectileSourcePosition;
        protected float _rotation;

        protected Matrix _localTransform =>
            Matrix.CreateTranslation(0, 0, 0f) *
            Matrix.CreateScale(1f, 1f, 1f) *
            Matrix.CreateRotationZ(_rotation) *
            Matrix.CreateTranslation(_position.X, _position.Y, 0f);

        private float _cooldownRemaining;
        private Random _random = new Random();

        public WeaponBase(
            Vector2 position,
            Vector2 projectileSourcePosition,
            float cooldown,
            float speed,
            float accuracy,
            Type projectileType)
        {
            _position = position;
            _projectileSourcePosition = projectileSourcePosition;
            _cooldown = cooldown;
            _speed = speed;
            _accuracy = accuracy;
            _projectileType = projectileType;
            _cooldownRemaining = 0;
        }

        public virtual void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_cooldownRemaining > 0)
            {
                _cooldownRemaining -= deltaTime;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Matrix parentTransform) { }

        public virtual void Fire(FactionType faction, Vector2 relativeVelocity, string solarSystemName, Matrix parentTransform)
        {
            Matrix globalTransform = _localTransform * parentTransform;
            MathUtilities.DecomposeMatrix(ref globalTransform, out Vector2 position, out float rotation, out Vector2 scale);

            if (_cooldownRemaining <= 0)
            {
                _cooldownRemaining = _cooldown;
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, rotation);

                float jitter = 1 - _accuracy / 100;
                float randomSpread = _random.NextFloat(-jitter, jitter) + _random.NextFloat(-jitter, jitter);
                Vector2 velocity = MathUtilities.FromPolar(rotation + randomSpread, _speed);

                Vector2 offset = Vector2.Transform(_projectileSourcePosition, aimQuat);
                EntityManager.Add((ProjectileBase)Activator.CreateInstance(
                    _projectileType,
                    faction,
                    position + offset,
                    velocity + relativeVelocity,
                    rotation,
                    solarSystemName));
            }
        }
    }
}
