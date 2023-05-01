using Microsoft.Xna.Framework;
using SpaceGame.Ships.Parts;
using SpaceGame.Weapons;

namespace SpaceGame.Ships
{
    public class TestShip2 : ShipBase
    {
        public TestShip2(FactionType faction, Vector2 spawnPosition, float spawnHeading)
            : base(faction,
                spawnPosition,
                spawnHeading,
                texture: Art.TestShip2,
                thrust: 200f,
                maxTurnRate: 2f,
                maneuveringThrust: 0.05f,
                maxVelocity: 100f,
                maxHealth: 1500f,
                maxShield: 100f,
                shieldRegen: 10f,
                scale: ScaleType.Half)
        {
            _weapons.Add(new TestWeapon(Vector2.Zero + new Vector2(Texture.Width / 2 * Scale, 0)));
            _thrusters.Add(new Thruster1(Art.Thruster1, new Vector2(-40f, -20), Vector2.Zero, 0.5f));
            _thrusters.Add(new Thruster1(Art.Thruster1, new Vector2(-40f, 20.5f), Vector2.Zero, 0.5f));
            _thrusters.Add(new Thruster1(Art.Thruster1, new Vector2(-50f, -8), Vector2.Zero, 0.75f));
            _thrusters.Add(new Thruster1(Art.Thruster1, new Vector2(-50f, 7.5f), Vector2.Zero, 0.75f));
        }
    }
}
