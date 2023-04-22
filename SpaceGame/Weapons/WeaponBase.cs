using Microsoft.Xna.Framework;
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

        private Vector2 _relativePosition;
        private float _cooldownRemaining;
        private static Random _random = new Random();

        public WeaponBase(
            Vector2 relativePosition,
            float cooldown,
            float speed,
            float accuracy,
            Type projectileType)
        {
            _relativePosition = relativePosition;
            _cooldown = cooldown;
            _speed = speed;
            _accuracy = accuracy;
            _projectileType = projectileType;
        }

        public void Fire(float heading, Vector2 relativeVelocity, Vector2 shipLocation)
        {
            if (_cooldownRemaining <= 0)
            {
                _cooldownRemaining = _cooldown;
                Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, heading);

                float jitter = 1 - _accuracy / 100;
                float randomSpread = _random.NextFloat(-jitter, jitter) + _random.NextFloat(-jitter, jitter);
                Vector2 vel = MathUtilities.FromPolar(heading + randomSpread, _speed);

                Vector2 offset = Vector2.Transform(_relativePosition, aimQuat);
                EntityManager.Add((ProjectileBase)Activator.CreateInstance(_projectileType, shipLocation + offset, vel + relativeVelocity));
            }

            if (_cooldownRemaining > 0)
            {
                _cooldownRemaining--;
            }
        }
    }
}
