using Microsoft.Xna.Framework;
using SpaceGame.Weapons;

namespace SpaceGame.Ships
{
    public class TestEnemyShip : ShipBase
    {
        public TestEnemyShip(Vector2 spawnPosition, float spawnHeading)
            : base(spawnPosition,
                spawnHeading,
                image: Art.TestEnemyShip,
                thrust: 300f,
                maxTurnRate: 1f,
                maneuveringThrust: 0.05f,
                maxVelocity: 1000f,
                scale: 0.5f,
                imageRotationOverride: MathHelper.ToRadians(90))
        {
            _weapons.Add(new TestWeapon(Vector2.Zero + new Vector2(_image.Width / 2, 0)));
        }
    }
}
