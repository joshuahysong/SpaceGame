using Microsoft.Xna.Framework;
using SpaceGame.Projectiles;

namespace SpaceGame.Weapons
{
    public class TestWeapon : WeaponBase
    {
        public TestWeapon(Vector2 position)
            : base(position,
                cooldown: 25f,
                speed: 2000f,
                accuracy: 99f,
                projectileType: typeof(TestProjectile))
        {
        }
    }
}
