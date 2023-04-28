using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;

namespace SpaceGame.ParticleEffects
{
    public interface IParticleEffect
    {
        public ParticleEffect ParticleEffect { get; }
        public Vector2 Position { get; set; }
        public Vector2 PositionOffset { get; set; }
        public Vector2 Rotation { get; set; }
        public bool IsExpired { get; set; }

        public void Update(GameTime gameTime);

        public void Draw(SpriteBatch spriteBatch);
    }
}
