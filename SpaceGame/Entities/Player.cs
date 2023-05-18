using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Behaviors;
using SpaceGame.Common;
using SpaceGame.Generators;
using SpaceGame.Scenes;
using SpaceGame.Scenes.Components;
using SpaceGame.Ships;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Entities
{
    public class Player : IFocusable, IEntity, IDisposable
    {
        public Vector2 Position => Ship?.Position ?? Vector2.Zero;
        public bool IsExpired => Ship?.IsExpired ?? false;
        public Vector2 WorldPosition => Ship == null ? Vector2.Zero : Ship.Position;

        public ShipBase Ship { get; }

        public string SelectedSolarSystemName { get; set; }

        public string CurrentSolarSystemName
        {
            get
            {
                return Ship.CurrentSolarSystemName;
            }
            set
            {
                Ship.CurrentSolarSystemName = value;
                CurrentSolarSystemNameChanged?.Invoke(Ship.CurrentSolarSystemName);
            }
        }

        public static event Action<string> CurrentSolarSystemNameChanged;

        private List<IEnumerator<int>> _behaviours = new();
        private float _angleToSelectedSolarSystem;
        private bool disposedValue;

        public Player(ShipBase ship, string solarSystemName)
        {
            Ship = ship;
            CurrentSolarSystemName = solarSystemName;

            UniverseMapScene.SolarSystemSelectionChanged += HandleSolarSystemSelectionChanged;
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!Ship.AreControlsLocked)
                HandleInput(deltaTime);

            if (_behaviours.Any())
                ApplyBehaviours();

            Ship.Update(gameTime, parentTransform);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform, bool drawMinimized = false)
        {
            Ship.Draw(spriteBatch, parentTransform, drawMinimized);
        }

        private void HandleInput(float deltaTime)
        {
            if (Input.IsKeyPressed(Keys.W) || Input.IsKeyPressed(Keys.Up))
            {
                Ship.ApplyForwardThrust();
            }
            if (Input.IsKeyPressed(Keys.A) || Input.IsKeyPressed(Keys.Left))
            {
                Ship.ApplyStarboardManeuveringThrusters();
            }
            else if (Input.IsKeyPressed(Keys.D) || Input.IsKeyPressed(Keys.Right))
            {
                Ship.ApplyPortManeuveringThrusters();
            }
            else if (Input.IsKeyPressed(Keys.S) || Input.IsKeyPressed(Keys.Down) || Input.IsKeyPressed(Keys.X))
            {
                Ship.RotateToRetro(deltaTime, Input.IsKeyPressed(Keys.X));
            }

            if (Input.IsKeyPressed(Keys.Space))
            {
                Ship.FireWeapons();
            }

            if (Input.WasKeyPressed(Keys.J) && !string.IsNullOrWhiteSpace(SelectedSolarSystemName))
            {
                JumpToSystem(deltaTime);
            }
        }

        public void HandleSolarSystemSelectionChanged(SolarSystem selectedSolarSystem)
        {
            SelectedSolarSystemName = selectedSolarSystem.Name;
            if (UniverseGenerator.SolarSystemLookup.TryGetValue(CurrentSolarSystemName, out var currentSolarSystem))
                _angleToSelectedSolarSystem = (selectedSolarSystem.MapLocation - currentSolarSystem.MapLocation).ToAngle();
        }

        private void ApplyBehaviours()
        {
            for (int i = 0; i < _behaviours.Count; i++)
            {
                if (!_behaviours[i].MoveNext())
                    _behaviours.RemoveAt(i--);
            }
        }

        private void JumpToSystem(float deltaTime)
        {
            var behavior = new JumpToSolarSystem(this, Ship, SelectedSolarSystemName, _angleToSelectedSolarSystem);
            _behaviours.Add(behavior.Perform(deltaTime).GetEnumerator());
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                UniverseMapScene.SolarSystemSelectionChanged -= HandleSolarSystemSelectionChanged;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
