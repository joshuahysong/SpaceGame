using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Entities;
using SpaceGame.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Ships
{
    public class ShipBase
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Heading;
        public float CurrentTurnRate;
        public bool IsManeuvering;
        public readonly Texture2D Texture;
        public readonly Color[] TextureData;

        public Matrix Transform => Matrix.CreateTranslation(new Vector3(-_origin, 0.0f))
            * Matrix.CreateRotationZ(Heading)
            * Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        public Rectangle BoundingRectangle => CalculateBoundingRectangle(_rectangle, Transform);

        protected float _thrust;
        protected float _maneuveringThrust;
        protected float _maxTurnRate;
        protected float _maxVelocity;
        protected List<WeaponBase> _weapons;

        private Vector2 _acceleration;

        private readonly Rectangle _rectangle;
        private readonly Vector2 _origin;

        public ShipBase(
            Vector2 spawnPosition,
            float spawnHeading,
            Texture2D texture,
            float thrust,
            float maneuveringThrust,
            float maxTurnRate,
            float maxVelocity)
        {
            Position = spawnPosition;
            Heading = spawnHeading;
            Texture = texture;
            _thrust = thrust;
            _maneuveringThrust = maneuveringThrust;
            _maxTurnRate = maxTurnRate;
            _maxVelocity = maxVelocity;
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            TextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(TextureData);
            _weapons = new List<WeaponBase>();
        }

        public void Update(GameTime gameTime, Matrix parentTransform)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Continue rotation until turn rate reaches zero to simulate slowing
            if (CurrentTurnRate > 0)
            {
                RotateClockwise(deltaTime);
            }
            else if (CurrentTurnRate < 0)
            {
                RotateCounterClockwise(deltaTime);
            }

            Velocity += _acceleration * deltaTime;

            // Cap velocity to max velocity
            if (Velocity.LengthSquared() > _maxVelocity * _maxVelocity)
            {
                Velocity.Normalize();
                Velocity *= _maxVelocity;
            }
            _acceleration.X = 0;
            _acceleration.Y = 0;
            Position += Velocity * deltaTime;

            if (!IsManeuvering)
            {
                if (CurrentTurnRate != 0)
                {
                    SlowDownManueveringThrust();
                }
            }
            IsManeuvering = false;
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Heading, _origin, 1f, SpriteEffects.None, 0);

            if (MainGame.IsDebugging)
            {
                Art.DrawLine(spriteBatch, Position, Position + Velocity, Color.Blue);
                Art.DrawLine(spriteBatch, Position, _maxVelocity, Heading, Color.Green);

                var color = IsColliding() ? Color.Red : Color.White;
                var boundingTexture = Art.CreateRectangle(BoundingRectangle.Width, BoundingRectangle.Height, Color.Transparent, color);
                spriteBatch.Draw(boundingTexture, BoundingRectangle, Color.White);
            }
        }

        public void FireWeapons()
        {
            _weapons.ForEach(weapon => weapon.Fire(Heading, Velocity, Position));
        }

        public void ApplyForwardThrust()
        {
            _acceleration.X += _thrust * (float)Math.Cos(Heading);
            _acceleration.Y += _thrust * (float)Math.Sin(Heading);
        }

        public void ApplyPortManeuveringThrusters()
        {
            CurrentTurnRate = CurrentTurnRate + _maneuveringThrust > _maxTurnRate
                ? _maxTurnRate
                : CurrentTurnRate + _maneuveringThrust;
            IsManeuvering = true;
        }

        public void ApplyStarboardManeuveringThrusters()
        {
            CurrentTurnRate = CurrentTurnRate - _maneuveringThrust < -_maxTurnRate
                ? -_maxTurnRate
                : CurrentTurnRate - _maneuveringThrust;
            IsManeuvering = true;
        }

        public void RotateToRetro(float deltaTime, bool IsBraking)
        {
            IsManeuvering = true;
            float movementHeading = Velocity.ToAngle();
            float retroHeading = movementHeading < 0 ? movementHeading + (float)Math.PI : movementHeading - (float)Math.PI;
            if (Heading != retroHeading && !IsWithinBrakingRange())
            {
                double retroDegrees = (retroHeading + Math.PI) * (180.0 / Math.PI);
                double headingDegrees = (Heading + Math.PI) * (180.0 / Math.PI);
                double turnRateDegrees = Math.PI * 2 * (CurrentTurnRate * deltaTime) / 100 * 360 * 2;
                turnRateDegrees = turnRateDegrees < 0 ? turnRateDegrees * -1 : turnRateDegrees;
                double retroOffset = headingDegrees < retroDegrees ? (headingDegrees + 360) - retroDegrees : headingDegrees - retroDegrees;

                double thrustMagnitude = Math.Round(CurrentTurnRate / _maneuveringThrust * CurrentTurnRate);
                thrustMagnitude = thrustMagnitude < 0 ? thrustMagnitude * -1 : thrustMagnitude;

                if (retroOffset >= 360 - turnRateDegrees || retroOffset <= turnRateDegrees)
                {
                    Heading = retroHeading;
                    CurrentTurnRate = 0;
                }
                else if (retroOffset > thrustMagnitude && 360 - retroOffset > thrustMagnitude)
                {
                    if (retroOffset < 180)
                    {
                        CurrentTurnRate = CurrentTurnRate - _maneuveringThrust < -_maxTurnRate ? -_maxTurnRate
                            : CurrentTurnRate - _maneuveringThrust;
                    }
                    else
                    {
                        CurrentTurnRate = CurrentTurnRate + _maneuveringThrust > _maxTurnRate ? _maxTurnRate
                            : CurrentTurnRate + _maneuveringThrust;
                    }
                }
                else
                {
                    SlowDownManueveringThrust();
                }
            }
            else if (IsBraking)
            {
                if (IsWithinBrakingRange())
                {
                    Velocity = Vector2.Zero;
                }
                else if (Heading == retroHeading)
                {
                    _acceleration.X += _thrust * (float)Math.Cos(Heading);
                    _acceleration.Y += _thrust * (float)Math.Sin(Heading);
                }
            }
        }

        private void RotateClockwise(float deltaTime)
        {
            Heading += CurrentTurnRate * deltaTime;
            if (Heading > Math.PI)
            {
                Heading = (float)-Math.PI;
            }
        }

        private void RotateCounterClockwise(float deltaTime)
        {
            Heading += CurrentTurnRate * deltaTime;
            if (Heading < -Math.PI)
            {
                Heading = (float)Math.PI;
            }
        }

        private void SlowDownManueveringThrust()
        {
            if (CurrentTurnRate < 0)
            {
                CurrentTurnRate = CurrentTurnRate + _maneuveringThrust > 0 ? 0 : CurrentTurnRate + _maneuveringThrust;
            }
            else if (CurrentTurnRate > 0)
            {
                CurrentTurnRate = CurrentTurnRate - _maneuveringThrust < 0 ? 0 : CurrentTurnRate - _maneuveringThrust;
            }
        }

        private bool IsWithinBrakingRange()
        {
            double brakingRange = _maxVelocity / 100;
            return Velocity.X < brakingRange && Velocity.X > -brakingRange && Velocity.Y < brakingRange && Velocity.Y > -brakingRange;
        }

        private static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        private bool IsColliding()
        {
            bool retval = false;

            // TODO REMOVE THIS TERRIBAD O(n2) IMPLEMENTATION
            foreach (var entity in EntityManager.Entities.Where(x => x is Enemy && ((Enemy)x).Ship != this))
            {
                var ship = ((Enemy)entity).Ship;
                if (BoundingRectangle.Intersects(ship.BoundingRectangle))
                {
                    if (HasIntersectingPixels(this, ship))
                    {
                        retval = true;
                    }
                    return retval;
                }
            }

            return retval;
        }

        public static bool HasIntersectingPixels(ShipBase shipA, ShipBase shipB)
        {
            var transformA = shipA.Transform;
            var widthA = shipA.Texture.Width;
            var heightA = shipA.Texture.Height;
            var dataA = shipA.TextureData;

            var transformB = shipB.Transform;
            var widthB = shipB.Texture.Width;
            var heightB = shipB.Texture.Height;
            var dataB = shipB.TextureData;

            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);
            for (int yA = 0; yA < heightA; yA++)
            {
                Vector2 posInB = yPosInB;
                for (int xA = 0; xA < widthA; xA++)
                {
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            return true;
                        }
                    }
                    posInB += stepX;
                }
                yPosInB += stepY;
            }
            return false;
        }
    }
}
