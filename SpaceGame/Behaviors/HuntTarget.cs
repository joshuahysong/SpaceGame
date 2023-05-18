using SpaceGame.Ships;
using System.Collections.Generic;

namespace SpaceGame.Behaviors
{
    public class HuntTarget
    {
        private ShipBase _agent;
        private ShipBase _target;

        public HuntTarget(ShipBase agent, ShipBase target)
        {
            _agent = agent;
            _target = target;
        }

        public IEnumerable<int> Perform()
        {
            while (true && _target != null)
            {
                float distanceToTarget = (_agent.Position - _target.Position).Length();
                double degreesToTarget = GetDegreesToTarget();
                if (distanceToTarget > 100)
                {
                    _agent.ApplyForwardThrust();
                }
                if (degreesToTarget > 175 && degreesToTarget < 185)
                {
                    _agent.FireWeapons();
                }
                if (degreesToTarget <= 178)
                {
                    _agent.ApplyPortManeuveringThrusters();
                }
                else if (degreesToTarget > 182)
                {
                    _agent.ApplyStarboardManeuveringThrusters();
                }
                else
                {
                    _agent.IsManeuvering = false;
                }
                yield return 0;
            }
        }

        private double GetDegreesToTarget()
        {
            float angleToTarget = (_agent.Position - _target.Position).ToAngle();
            double degreesToTarget = angleToTarget.ToDegrees();
            double headingDegrees = _agent.Heading.ToDegrees();
            return headingDegrees < degreesToTarget ? headingDegrees + 360 - degreesToTarget : headingDegrees - degreesToTarget;
        }
    }
}
