using Microsoft.Xna.Framework;
using SpaceGame.Common;
using SpaceGame.Entities;
using SpaceGame.Ships;
using System;
using System.Collections.Generic;

namespace SpaceGame.Behaviors
{
    public class JumpToSolarSystem
    {
        private readonly IEntity _agent;
        private readonly ShipBase _agentShip;
        private readonly string _targetSolarSystemName;
        private readonly float _angleToTarget;

        private float _jumpThrust = 2000f;

        private bool _isManeuveringToStartJump;
        private bool _isSlowingToFinishJump;

        public JumpToSolarSystem(IEntity agent, ShipBase agentShip, string targetSolarSystemName, float angleToTarget)
        {
            _agent = agent;
            _agentShip = agentShip;
            _targetSolarSystemName = targetSolarSystemName;
            _angleToTarget = angleToTarget;
            _isManeuveringToStartJump = true;
        }

        public IEnumerable<int> Perform(float deltaTime)
        {
            var isJumping = true;
            while (isJumping)
            {
                if (_isManeuveringToStartJump && _agentShip.Velocity != Vector2.Zero)
                {
                    _agentShip.RotateToRetro(deltaTime, true);
                    yield return 0;
                }
                else
                {
                    if (_agentShip.Heading != _angleToTarget)
                    {
                        _agentShip.RotateToHeading(_angleToTarget, deltaTime, false);
                        yield return 0;
                    }
                    else if (_isSlowingToFinishJump && !_agentShip.IsBelowMaxVelocity())
                    {
                        _agentShip.ApplyRetroThrust(_jumpThrust);
                        yield return 0;
                    }
                    else if (_isSlowingToFinishJump && _agentShip.IsBelowMaxVelocity())
                    {
                        _agentShip.IsJumping = false;
                        isJumping = false;
                    }
                    else if (_agentShip.IsJumping && _agentShip.Velocity.LengthSquared() > Constants.JumpSpeed * Constants.JumpSpeed)
                    {
                        _isSlowingToFinishJump = true;
                        _agent.CurrentSolarSystemName = _targetSolarSystemName;
                        var distanceFromCenter = 4000;
                        _agentShip.Position = -new Vector2
                            (
                                distanceFromCenter * (float)Math.Cos(_angleToTarget),
                                distanceFromCenter * (float)Math.Sin(_angleToTarget)
                            );

                        if (_agent is Player)
                            ((Player)_agent).SelectedSolarSystemName = null;

                        yield return 0;
                    }
                    else
                    {
                        _agentShip.IsJumping = true;
                        _isManeuveringToStartJump = false;
                        _agentShip.ApplyForwardThrust(_jumpThrust);
                        yield return 0;
                    }
                }
            }
        }
    }
}
