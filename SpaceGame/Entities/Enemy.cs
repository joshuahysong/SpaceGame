using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
using SpaceGame.Ships;
using System.Collections.Generic;

namespace SpaceGame.Entities
{
    public class Enemy : IEntity
    {
        public Vector2 Position => Ship?.Position ?? Vector2.Zero;
        public Vector2 TileCoordinates { get; set; }
        public bool IsExpired => Ship?.IsExpired ?? false;

        public ShipBase Ship { get; set; }
        public ShipBase Target { get; set; }

        private readonly List<IEnumerator<int>> _behaviours = new();

        public Enemy(ShipBase ship)
        {
            Ship = ship;
            Target = MainGame.Player.Ship;
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
            while (true && Target != null)
            {
                float distanceToTarget = (Ship.Position - Target.Position).Length();
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
            float angleToTarget = (Ship.Position - Target.Position).ToAngle();
            double degreesToTarget = angleToTarget.ToDegrees();
            double headingDegrees = Ship.Heading.ToDegrees();
            return headingDegrees < degreesToTarget ? headingDegrees + 360 - degreesToTarget : headingDegrees - degreesToTarget;
        }
    }
}
