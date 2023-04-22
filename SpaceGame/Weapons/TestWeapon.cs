using Microsoft.Xna.Framework;
using SpaceGame.Projectiles;

namespace SpaceGame.Weapons
{
    public class TestWeapon : WeaponBase
    {
        public TestWeapon(Vector2 relativePosition)
            : base(relativePosition,
                cooldown: 25f,
                speed: 2000f,
                accuracy: 99f,
                projectileType: typeof(TestProjectile))
        {
        }
    }
}
