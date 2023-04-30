using Microsoft.Xna.Framework;
using SpaceGame.Ships.Parts;
using SpaceGame.Weapons;

namespace SpaceGame.Ships
{
    public class TestShip1 : ShipBase
    {
        public TestShip1(FactionType faction, Vector2 spawnPosition, float spawnHeading)
            : base(faction,
                spawnPosition,
                spawnHeading,
                texture: Art.TestShip1,
                thrust: 300f,
                maxTurnRate: 2f,
                maneuveringThrust: 0.05f,
                maxVelocity: 300f,
                maxHealth: 1000f,
                maxShield: 1000f,
                shieldRegen: 10f,
                scale: ScaleType.Half)
        {
            _weapons.Add(new TestWeapon(new Vector2(Texture.Width / 2 * Scale, 0)));
            _thrusters.Add(new Thruster1(Art.Thruster1, new Vector2(-29.5f, -13), Vector2.Zero, ScaleType.Half));
            _thrusters.Add(new Thruster1(Art.Thruster1, new Vector2(-29.5f, 13.5f), Vector2.Zero, ScaleType.Half));
        }
    }
}
