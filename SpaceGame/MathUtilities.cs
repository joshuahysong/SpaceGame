using Microsoft.Xna.Framework;
using System;

namespace SpaceGame
{
    public class MathUtilities
    {
        public static Vector2 FromPolar(float angle, float magnitude)
        {
            return magnitude * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static void DecomposeMatrix(ref Matrix matrix, out Vector2 position, out float rotation, out Vector2 scale)
        {
            matrix.Decompose(out Vector3 scale3, out Quaternion rotationQ, out Vector3 position3);
            Vector2 direction = Vector2.Transform(Vector2.UnitX, rotationQ);
            rotation = direction.ToAngle();
            position = new Vector2(position3.X, position3.Y);
            scale = new Vector2(scale3.X, scale3.Y);
        }
    }
}
