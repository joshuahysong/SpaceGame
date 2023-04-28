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
    public class TestParticleEffect : IParticleEffect
    {
        public ParticleEffect ParticleEffect { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Rotation { get; set; }
        public bool IsExpired { get; set; }

        private float _particleAliveTime = 0;

        public TestParticleEffect(Vector2 position)
        {
            Position = position;
            _particleAliveTime = 1f;

            var textureRegion = new TextureRegion2D(Art.Pixel);
            ParticleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = position,
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(2),
                        Profile.Circle(10f, Profile.CircleRadiation.Out))
                    {
                        AutoTrigger = true,
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 50f),
                            Quantity = 3,
                            Rotation = new Range<float>(-10f, 10f),
                            Scale = new Range<float>(1.0f, 1.0f)
                        },
                        Modifiers =
                        {
                            new AgeModifier
                            {
                                Interpolators =
                                {
                                    new ColorInterpolator
                                    {
                                        StartValue = Color.Cyan.ToHsl(),
                                        EndValue = Color.Cyan.ToHsl()
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

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            ParticleEffect.Position = Position;
            ParticleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            if (_particleAliveTime > 0)
            {
                _particleAliveTime -= 0.1f;
            }
            else if (_particleAliveTime <= 0)
            {
                ParticleEffect.Emitters.First().AutoTrigger = false;
                if (ParticleEffect.ActiveParticles <= 0)
                {
                    ParticleEffect.Dispose();
                    IsExpired = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(ParticleEffect);
        }
    }
}
