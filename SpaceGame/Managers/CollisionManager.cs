using Microsoft.Xna.Framework;
using SpaceGame.Common;
using SpaceGame.Ships;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame.Managers
{
    public static class CollisionManager
    {
        public static Dictionary<Guid, List<ICollidable>> Collisions { get; private set; }

        public static int Count => _collidables?.Count ?? 0;

        private static List<ICollidable> _collidables = new();

        public static void Initialize()
        {
            _collidables = new List<ICollidable>();
        }

        public static void Add(ICollidable collidable)
        {
            _collidables.Add(collidable);
        }

        public static void Update()
        {
            _collidables = _collidables.Where(x => !x.IsExpired).ToList();
            GetCollisions();
        }

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
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

        public static void GetCollisions()
        {
            var collisions = new Dictionary<Guid, List<ICollidable>>();
            var collidablesToCheck = _collidables.Where(x => x is ShipBase).ToList();
            foreach (var collidableToCheck in collidablesToCheck)
            {
                var entityCollisions = new List<ICollidable>();
                foreach (var entity in _collidables.Where(x => x != collidableToCheck &&
                    collidableToCheck.Faction != x.Faction &&
                    collidableToCheck.CurrentSolarSystemName == x.CurrentSolarSystemName))
                {
                    if (collidableToCheck.BoundingRectangle.Intersects(entity.BoundingRectangle))
                    {
                        if (HasIntersectingPixels(collidableToCheck, entity))
                        {
                            entityCollisions.Add(entity);
                        }
                    }
                }
                collisions[collidableToCheck.Id] = entityCollisions;
            }

            Collisions = collisions;
        }

        public static bool HasIntersectingPixels(ICollidable collidableEntityA, ICollidable collidableEntityB)
        {
            var transformA = collidableEntityA.Transform;
            var widthA = (int)Math.Floor(collidableEntityA.Texture.Width * collidableEntityA.Scale);
            var heightA = (int)Math.Floor(collidableEntityA.Texture.Height * collidableEntityA.Scale);
            var dataA = collidableEntityA.TextureData;

            var transformB = collidableEntityB.Transform;
            var widthB = (int)Math.Floor(collidableEntityB.Texture.Width * collidableEntityB.Scale);
            var heightB = (int)Math.Floor(collidableEntityB.Texture.Height * collidableEntityB.Scale);
            var dataB = collidableEntityB.TextureData;

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
