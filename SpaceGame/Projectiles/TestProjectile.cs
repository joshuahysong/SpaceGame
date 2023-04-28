using Microsoft.Xna.Framework;
using SpaceGame.Managers;
using SpaceGame.ParticleEffects;

namespace SpaceGame.Projectiles
{
    public class TestProjectile : ProjectileBase
    {
        public TestProjectile(FactionType faction, Vector2 position, Vector2 velocity)
            : base(faction,
                  position,
                  velocity,
                  texture: Art.Bullet,
                  timeToLiveInSeconds: 5)
        {
        }

        public override int PerformHitEffect()
        {
            ParticleEffectsManager.Add(new TestParticleEffect(Position));
            return 50;
        }
    }
}
