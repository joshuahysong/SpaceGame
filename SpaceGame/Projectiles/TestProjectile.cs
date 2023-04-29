﻿using Microsoft.Xna.Framework;
using SpaceGame.Managers;
using SpaceGame.ParticleEffects;

namespace SpaceGame.Projectiles
{
    public class TestProjectile : ProjectileBase
    {
        public TestProjectile(FactionType faction, Vector2 position, Vector2 velocity, float heading)
            : base(faction,
                  position,
                  velocity,
                  heading,
                  texture: Art.OrangeLaser,
                  timeToLiveInSeconds: 5,
                  scale: 0.5f)
        {
        }

        public override int PerformHitEffect()
        {
            ParticleEffectsManager.Add(new TestParticleEffect(Position));
            return 50;
        }
    }
}
