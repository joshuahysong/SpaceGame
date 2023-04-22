using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Projectiles;
using System;

namespace SpaceGame.Weapons
{
    public class WeaponBase : Entity
    {
        protected float _cooldown;
        protected float _speed;
        protected float _accuracy;
        protected Type _projectileType;

        private float _cooldownRemaining;
        private static Random _random = new Random();

        public WeaponBase(
            Vector2 position,
            float cooldown,
            float speed,
            float accuracy,
            Type projectileType)
        {
            Position = position;
            _cooldown = cooldown;
            _speed = speed;
            _accuracy = accuracy;
            _projectileType = projectileType;
        }

        public override void Update(GameTime gameTime, Matrix parentTransform) { }

        public override void Draw(SpriteBatch spriteBatch, Matrix parentTransform) { }

        public void Fire(float heading, Vector2 relativeVelocity, Vector2 shipLocation)
        {
            if (_cooldownRemaining <= 0)
            {
                _cooldownRemaining = _cooldown;
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, heading);

                float jitter = 1 - _accuracy / 100;
                float randomSpread = _random.NextFloat(-jitter, jitter) + _random.NextFloat(-jitter, jitter);
                Vector2 vel = MathUtilities.FromPolar(heading + randomSpread, _speed);

                Vector2 offset = Vector2.Transform(Position, aimQuat);
                EntityManager.Add((ProjectileBase)Activator.CreateInstance(_projectileType, shipLocation + offset, vel + relativeVelocity));
            }

            if (_cooldownRemaining > 0)
            {
                _cooldownRemaining--;
            }
        }
    }
}
