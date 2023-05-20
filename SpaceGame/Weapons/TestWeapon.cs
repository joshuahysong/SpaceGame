using Microsoft.Xna.Framework;
using SpaceGame.Projectiles;

namespace SpaceGame.Weapons
{
    public class TestWeapon : WeaponBase
    {
        public TestWeapon(Vector2 position)
            : base(position,
                projectileSourcePosition: Vector2.Zero,
                cooldown: 0.5f,
                speed: 2000f,
                accuracy: 99f,
                projectileType: typeof(OrangeLaser))
        {
        }
    }
}
