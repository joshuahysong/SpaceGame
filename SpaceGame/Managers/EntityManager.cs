using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Managers
{
    public static class EntityManager
    {
        public static List<IEntity> Entities = new List<IEntity>();

        private static List<IEntity> _addedEntities = new List<IEntity>();
        private static bool _isUpdating;

        public static int Count => Entities.Count;

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

        public static void Update(GameTime gameTime, Matrix parentTransform)
        {
            _isUpdating = true;

            foreach (IEntity entity in Entities)
            {
                entity.Update(gameTime, parentTransform);
                UpdateEntityTileCoordinates(entity);
            }

            _isUpdating = false;

            foreach (IEntity entity in _addedEntities)
            {
                AddEntity(entity);
            }

            _addedEntities.Clear();

            // remove any expired entities.
            Entities = Entities.Where(x => !x.IsExpired).ToList();
        }

        public static void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            foreach (IEntity entity in Entities)
            {
                entity.Draw(spriteBatch, parentTransform);
            }
        }

        private static void AddEntity(IEntity entity)
        {
            Entities.Add(entity);
        }

        private static void UpdateEntityTileCoordinates(IEntity entity)
        {
            entity.TileCoordinates = new Vector2(
                (float)Math.Floor(entity.Position.X / SpaceScene.TileSize),
                (float)Math.Floor(entity.Position.Y / SpaceScene.TileSize));
        }
    }
}
