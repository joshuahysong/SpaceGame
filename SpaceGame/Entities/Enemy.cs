using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Behaviors;
using SpaceGame.Common;
using SpaceGame.Ships;
using System.Collections.Generic;

namespace SpaceGame.Entities
{
    public class Enemy : IEntity
    {
        public Vector2 Position => Ship?.Position ?? Vector2.Zero;
        public bool IsExpired => Ship?.IsExpired ?? false;

        public ShipBase Ship { get; }

        public string CurrentSolarSystemName
        {
            get
            {
                return Ship.CurrentSolarSystemName;
            }
            set
            {
                Ship.CurrentSolarSystemName = value;
            }
        }

        private List<IEnumerator<int>> _behaviours = new();

        public Enemy(ShipBase ship, ShipBase target, string solarSystemName)
        {
            Ship = ship;
            CurrentSolarSystemName = solarSystemName;
            var huntTargetBehavior = new HuntTarget(ship, target);
            AddBehavior(huntTargetBehavior.Perform());
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            ApplyBehaviours();
            Ship.Update(gameTime, parentTransform);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform, bool drawMinimized = false)
        {
            Ship.Draw(spriteBatch, parentTransform, drawMinimized);
        }

        private void AddBehavior(IEnumerable<int> behaviour)
        {
            _behaviours.Add(behaviour.GetEnumerator());
        }

        private void ApplyBehaviours()
        {
            for (int i = 0; i < _behaviours.Count; i++)
            {
                if (!_behaviours[i].MoveNext())
                    _behaviours.RemoveAt(i--);
            }
        }
    }
}
