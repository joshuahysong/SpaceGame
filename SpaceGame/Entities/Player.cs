using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Common;
using SpaceGame.Ships;
using System;

namespace SpaceGame.Entities
{
    public class Player : IFocusable, IAgent
    {
        public Vector2 Position => Ship?.Position ?? Vector2.Zero;
        public bool IsExpired => Ship?.IsExpired ?? false;
        public Vector2 WorldPosition => Ship == null ? Vector2.Zero : Ship.Position;

        public ShipBase Ship { get; }

        public string CurrentSolarSystemName {
            get
            {
                return _currentSolarSystemName;
            }
            set
            {
                _currentSolarSystemName = value;
                CurrentSolarSystemNameChanged?.Invoke(_currentSolarSystemName);
            }
        }

        public static event Action<string> CurrentSolarSystemNameChanged;

        private string _currentSolarSystemName;

        public Player(ShipBase ship)
        {
            Ship = ship;
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!Ship.AreControlsLocked)
                HandleInput(deltaTime);
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
        }
    }
}
