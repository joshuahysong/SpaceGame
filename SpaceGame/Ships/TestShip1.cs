using Microsoft.Xna.Framework;
using SpaceGame.ParticleEffects;
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
                maxHealth: 10000,
                scale: ScaleType.Half)
        {
            _weapons.Add(new TestWeapon(new Vector2(Texture.Width / 2, 0)));
            //_thrustEffects.Add(new TestThrustParticleEffect(new Vector2(-25, -14)));
            //_thrustEffects.Add(new TestThrustParticleEffect(new Vector2(-25, 14)));
        }
    }
}
