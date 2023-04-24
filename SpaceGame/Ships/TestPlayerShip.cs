using Microsoft.Xna.Framework;
using SpaceGame.Weapons;

namespace SpaceGame.Ships
{
    public class TestPlayerShip : ShipBase
    {
        public TestPlayerShip(Vector2 spawnPosition, float spawnHeading)
            : base(spawnPosition,
                spawnHeading,
                texture: Art.TestPlayerShip,
                thrust: 300f,
                maxTurnRate: 2f,
                maneuveringThrust: 0.05f,
                maxVelocity: 1000f)
        {
            _weapons.Add(new TestWeapon(new Vector2(Texture.Width / 2, 0)));
        }
    }
}
