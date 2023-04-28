using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.ParticleEffects;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Managers
{
    public class ParticleEffectsManager
    {
        public static int Count => _effects.Count;

        private static List<IParticleEffect> _effects = new();
        private static List<IParticleEffect> _addedEffects = new();
        private static bool _isUpdating;

        public static void Add(IParticleEffect effect)
        {
            if (!_isUpdating)
            {
                AddEffect(effect);
            }
            else
            {
                _addedEffects.Add(effect);
            }
        }

        public static void Update(GameTime gameTime, Matrix parentTransform)
        {
            _isUpdating = true;

            foreach (IParticleEffect effect in _effects)
            {
                effect.Update(gameTime, parentTransform);
            }

            _isUpdating = false;

            foreach (IParticleEffect effect in _addedEffects)
            {
                AddEffect(effect);
            }

            _addedEffects.Clear();

            // remove any expired entities.
            _effects = _effects.Where(x => !x.IsExpired).ToList();
        }

        public static void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            foreach (IParticleEffect effect in _effects)
            {
                effect.Draw(spriteBatch, parentTransform);
            }
        }

        private static void AddEffect(IParticleEffect effect)
        {
            _effects.Add(effect);
        }
    }
}
