using Microsoft.Xna.Framework;
using SpaceGame.Weapons;

namespace SpaceGame.Ships
{
    public class TestPlayerShip : ShipBase
    {
        public TestPlayerShip(Vector2 spawnPosition, float spawnHeading) : base(spawnPosition, spawnHeading)
        {
            _image = Art.TestPlayerShip;
            _thrust = 300f;
            _maxTurnRate = 1f;
            _maneuveringThrust = 0.05f;
            _maxVelocity = 1000f;
            _imageRotationOverride = MathHelper.ToRadians(90);
            _weapons.Add(new TestWeapon(Vector2.Zero + new Vector2(_image.Width / 2, 0)));
        }
    }
}
