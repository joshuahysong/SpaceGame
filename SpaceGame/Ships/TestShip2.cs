using Microsoft.Xna.Framework;
using SpaceGame.ParticleEffects;
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
                maxHealth: 10000,
                scale: ScaleType.Half)
        {
            _weapons.Add(new TestWeapon(Vector2.Zero + new Vector2(Texture.Width / 2, 0)));
            //_thrustEffects.Add(new TestThrustParticleEffect(new Vector2(-37, -20)));
            //_thrustEffects.Add(new TestThrustParticleEffect(new Vector2(-37, 20)));
            //_thrustEffects.Add(new TestThrustParticleEffect(new Vector2(-45, -8)));
            //_thrustEffects.Add(new TestThrustParticleEffect(new Vector2(-45, 8)));
        }
    }
}
