using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private List<IEnumerator<int>> _behaviours;
        private ShipBase _target;

        public Enemy(ShipBase ship, ShipBase target)
        {
            Ship = ship;
            _target = target;
            _behaviours = new();
            //AddBehavior(HuntPlayer());
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            ApplyBehaviours();
            Ship.Update(gameTime, parentTransform);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            Ship.Draw(spriteBatch, parentTransform);
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
                {
                    _behaviours.RemoveAt(i--);
                }
            }
        }

        private IEnumerable<int> HuntPlayer()
        {
            while (true && _target != null)
            {
                float distanceToTarget = (Ship.Position - _target.Position).Length();
                double degreesToTarget = GetDegreesToTarget();
                if (distanceToTarget > 100)
                {
                    Ship.ApplyForwardThrust();
                }
                if (degreesToTarget > 175 && degreesToTarget < 185)
                {
                    Ship.FireWeapons();
                }
                if (degreesToTarget <= 178)
                {
                    Ship.ApplyPortManeuveringThrusters();
                }
                else if (degreesToTarget > 182)
                {
                    Ship.ApplyStarboardManeuveringThrusters();
                }
                else
                {
                    Ship.IsManeuvering = false;
                }
                yield return 0;
            }
        }

        private double GetDegreesToTarget()
        {
            float angleToTarget = (Ship.Position - _target.Position).ToAngle();
            double degreesToTarget = angleToTarget.ToDegrees();
            double headingDegrees = Ship.Heading.ToDegrees();
            return headingDegrees < degreesToTarget ? headingDegrees + 360 - degreesToTarget : headingDegrees - degreesToTarget;
        }
    }
}
