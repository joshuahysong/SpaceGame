using Microsoft.Xna.Framework;
using System;

namespace SpaceGame
{
    public static class Extensions
    {
        public static float ToAngle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static double ToDegrees(this float radians)
        {
            return (radians + Math.PI) * (180.0 / Math.PI);
        }

        public static float NextFloat(this Random random, float minValue, float maxValue)
        {
            return (float)random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
