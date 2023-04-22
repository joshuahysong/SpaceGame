using Microsoft.Xna.Framework;
using SpaceGame.Weapons;

namespace SpaceGame.Ships
{
    public class TestPlayerShip : ShipBase
    {
        public TestPlayerShip(Vector2 spawnPosition, float spawnHeading)
            : base(spawnPosition,
                spawnHeading,
                image: Art.TestPlayerShip,
                thrust: 300f,
                maxTurnRate: 1f,
                maneuveringThrust: 0.05f,
                maxVelocity: 1000f,
                scale: 0.5f,
                imageRotationOverride: MathHelper.ToRadians(90))
        {
            _weapons.Add(new TestWeapon(Vector2.Zero + new Vector2(_image.Width / 2, 0) * _scale));
        }
    }
}
