using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

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

        public static string WrapText(this string text, SpriteFont font, int maxWidth)
        {
            if (font.MeasureString(text).X < maxWidth)
                return text;

            var words = text.Split(' ');
            var wrappedText = new StringBuilder();
            var linewidth = 0f;
            var spaceWidth = font.MeasureString(" ").X;
            for (int i = 0; i < words.Length; ++i)
            {
                var size = font.MeasureString(words[i]);
                if (linewidth + size.X < maxWidth)
                {
                    linewidth += size.X + spaceWidth;
                }
                else
                {
                    wrappedText.Append('\n');
                    linewidth = size.X + spaceWidth;
                }
                wrappedText.Append(words[i]);
                wrappedText.Append(' ');
            }

            return wrappedText.ToString();
        }
    }
}
