using Microsoft.Xna.Framework;
using SpaceGame.Managers;
using SpaceGame.ParticleEffects;

namespace SpaceGame.Projectiles
{
    public class GreenBullet : ProjectileBase
    {
        public GreenBullet(FactionType faction, Vector2 position, Vector2 velocity, float heading, string solarSystemName)
            : base(faction,
                  position,
                  velocity,
                  heading,
                  texture: Art.Projectiles.GreenBullet,
                  timeToLiveInSeconds: 5,
                  solarSystemName,
                  scale: ScaleType.Half)
        {
        }

        public override float PerformHitEffect()
        {
            ParticleEffectsManager.Add(new TestParticleEffect(Position, CurrentSolarSystemName));
            return 50f;
        }
    }
}
