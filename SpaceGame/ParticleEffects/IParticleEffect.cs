using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using SpaceGame.Common;

namespace SpaceGame.ParticleEffects
{
    public interface IParticleEffect : IEntity
    {
        public ParticleEffect ParticleEffect { get; }
        public Vector2 Rotation { get; }
    }
}
