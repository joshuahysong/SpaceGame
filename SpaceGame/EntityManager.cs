using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame
{
    public static class EntityManager
    {
        private static List<IEntity> _entities = new List<IEntity>();
        private static List<IEntity> _addedEntities = new List<IEntity>();
        private static bool _isUpdating;

        public static int Count => _entities.Count;

        public static void Add(IEntity entity)
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

        private static void AddEntity(IEntity entity)
        {
            _entities.Add(entity);
        }

        public static void Update(GameTime gameTime, Matrix parentTransform)
        {
            _isUpdating = true;

            foreach (IEntity entity in _entities)
            {
                entity.Update(gameTime, parentTransform);
            }

            _isUpdating = false;

            foreach (IEntity entity in _addedEntities)
            {
                AddEntity(entity);
            }

            _addedEntities.Clear();

            // remove any expired entities.
            _entities = _entities.Where(x => !x.IsExpired).ToList();
        }

        public static void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            foreach (IEntity entity in _entities)
            {
                entity.Draw(spriteBatch, parentTransform);
            }
        }
    }
}
