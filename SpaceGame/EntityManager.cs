using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame
{
    public static class EntityManager
    {
        private static List<Entity> _entities = new List<Entity>();
        private static List<Entity> _addedEntities = new List<Entity>();
        private static bool _isUpdating;

        public static int Count => _entities.Count;

        public static void Add(Entity entity)
        {
            if (!_isUpdating)
            {
                AddEntity(entity);
            }
            else
            {
                _addedEntities.Add(entity);
            }
        }

        private static void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public static void Update(GameTime gameTime, Matrix parentTransform)
        {
            _isUpdating = true;

            foreach (Entity entity in _entities)
            {
                entity.Update(gameTime, parentTransform);
            }

            _isUpdating = false;

            foreach (Entity entity in _addedEntities)
            {
                AddEntity(entity);
            }

            _addedEntities.Clear();

            // remove any expired entities.
            _entities = _entities.Where(x => !x.IsExpired).ToList();
        }

        public static void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            foreach (Entity entity in _entities)
            {
                entity.Draw(spriteBatch, parentTransform);
            }
        }
    }
}
