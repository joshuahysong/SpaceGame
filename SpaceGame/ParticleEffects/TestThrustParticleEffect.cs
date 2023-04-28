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
        public Vector2 Rotation { get; set; }
        public bool IsExpired { get; set; }

        private Matrix _localTransform =>
            Matrix.CreateTranslation(0, 0, 0f) *
            Matrix.CreateScale(1f, 1f, 1f) *
            Matrix.CreateRotationZ(Rotation.ToAngle()) *
            Matrix.CreateTranslation(Position.X, Position.Y, 0f);

        public TestThrustParticleEffect(Vector2 position)
        {
            Position = position;

            var textureRegion = new TextureRegion2D(Art.Pixel);
            ParticleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = position,
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(0.05),
                        Profile.Line(new Vector2(0,0), 10))
                    {
                        AutoTrigger = false,
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(-100f, 100f),
                            Quantity = 10,
                            Rotation = new Range<float>(-10f, 10f),
                            Scale = new Range<float>(1f, 1.5f)
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

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            Matrix globalTransform = _localTransform * parentTransform;
            MathUtilities.DecomposeMatrix(ref globalTransform, out Vector2 position, out float rotation, out Vector2 scale);

            ParticleEffect.Position = position;
            ParticleEffect.Emitters.First().Profile = Profile.Line(Rotation, 10);
            ParticleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            Matrix globalTransform = _localTransform * parentTransform;
            MathUtilities.DecomposeMatrix(ref globalTransform, out Vector2 position, out float rotation, out Vector2 scale);

            ParticleEffect.Position = position;
            spriteBatch.Draw(ParticleEffect);
        }
    }
}
