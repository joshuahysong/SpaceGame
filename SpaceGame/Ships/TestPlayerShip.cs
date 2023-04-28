using Microsoft.Xna.Framework;
using SpaceGame.ParticleEffects;
using SpaceGame.Weapons;

namespace SpaceGame.Ships
{
    public class TestPlayerShip : ShipBase
    {
        public TestPlayerShip(Vector2 spawnPosition, float spawnHeading)
            : base(FactionType.Player,
                spawnPosition,
                spawnHeading,
                texture: Art.TestPlayerShip,
                thrust: 300f,
                maxTurnRate: 2f,
                maneuveringThrust: 0.05f,
                maxVelocity: 300f,
                maxHealth: 10000,
                scale: ScaleType.Half)
        {
            _weapons.Add(new TestWeapon(new Vector2(Texture.Width / 2, 0)));
            _thrustEffects.Add(new TestThrustParticleEffect(Position, new Vector2(-25, -14), Velocity));
            _thrustEffects.Add(new TestThrustParticleEffect(Position, new Vector2(-25, 14), Velocity));
        }
    }
}
