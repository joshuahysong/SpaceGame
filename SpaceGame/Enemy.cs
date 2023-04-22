using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Ships;
using System.Collections.Generic;

namespace SpaceGame
{
    public class Enemy : Entity
    {
        public ShipBase Ship { get; set; }
        public Entity Target { get; set; }
        public override Vector2 Position => Ship.Position;

        private readonly List<IEnumerator<int>> _behaviours = new();

        public Enemy(ShipBase ship)
        {
            Ship = ship;
            Position = ship.Position;
            Target = MainGame.Player;
            AddBehavior(HuntPlayer());
        }

        public override void Update(GameTime gameTime, Matrix parentTransform)
        {
            ApplyBehaviours();
            Position = Ship.Position;
            Ship.Update(gameTime, parentTransform);
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
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
                float distanceToTarget = (Position - Target.Position).Length();
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
            float angleToTarget = (Position - Target.Position).ToAngle();
            double degreesToTarget = angleToTarget.ToDegrees();
            double headingDegrees = Ship.Heading.ToDegrees();
            return headingDegrees < degreesToTarget ? (headingDegrees + 360) - degreesToTarget : headingDegrees - degreesToTarget;
        }
    }
}
