﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceGame.Managers;
using SpaceGame.Ships;
using System;

namespace SpaceGame.Entities
{
    public class Player : IEntity, IFocusable
    {
        public Vector2 Position => Ship?.Position ?? Vector2.Zero;
        public Vector2 TileCoordinates { get; set; }
        public bool IsExpired => Ship?.IsExpired ?? false;

        public Vector2 WorldPosition => Ship == null ? Vector2.Zero : Ship.Position;

        public ShipBase Ship { get; set; }

        public Player(ShipBase ship)
        {
            Ship = ship;
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            HandleInput(deltaTime);
            Ship.Update(gameTime, parentTransform);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            Ship.Draw(spriteBatch, parentTransform);
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
