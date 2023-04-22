using Microsoft.Xna.Framework;
using SpaceGame.Projectiles;

namespace SpaceGame.Weapons
{
    public class TestWeapon : WeaponBase
    {
        public TestWeapon(Vector2 position) : base(position)
        {
            _cooldown = 25f;
            _speed = 2000f;
            _accuracy = 99f;
            _projectileType = typeof(TestProjectile);
        }
    }
}
