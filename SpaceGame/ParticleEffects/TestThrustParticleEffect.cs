using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.ParticleEffects
{
    public class TestThrustParticleEffect : IParticleEffect
    {
        public ParticleEffect ParticleEffect { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 PositionOffset { get; set; }
        public Vector2 Rotation { get; set; }
        public bool IsExpired { get; set; }

        public TestThrustParticleEffect(
            Vector2 position,
            Vector2 positionOffset,
            Vector2 rotation)
        {
            Position = position;
            PositionOffset = positionOffset;
            Rotation = rotation;

            var textureRegion = new TextureRegion2D(Art.Pixel);
            ParticleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = position + positionOffset,
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(0.1),
                        Profile.Spray(-rotation, 0.5f))
                    {
                        AutoTrigger = false,
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 100f),
                            Quantity = 10,
                            Rotation = new Range<float>(-10f, 10f),
                            Scale = new Range<float>(1.5f, 1.5f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = Color.Red.ToHsl(),
                                        EndValue = Color.Yellow.ToHsl()
                                    }
                                }
                            },
                            new OpacityFastFadeModifier(),
                            new RotationModifier {RotationRate = -2.1f}
                        }
                    }
                }
            };

        }

        public void Update(GameTime gameTime)
        {
            ParticleEffect.Position = Position + PositionOffset;
            ParticleEffect.Emitters.First().Profile = Profile.Spray(Rotation, 0.5f);
            ParticleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ParticleEffect);
        }
    }
}
