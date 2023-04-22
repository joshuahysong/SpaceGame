using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceGame
{
    public static class Art
    {
        #region Backgrounds
        public static Texture2D Background { get; set; }
        #endregion

        #region Ships
        public static Texture2D TestShip { get; set; }
        #endregion

        #region Projectiles
        public static Texture2D Bullet { get; set; }
        #endregion

        #region Fonts
        public static SpriteFont DebugFont { get; private set; }
        public static SpriteFont UIFont { get; private set; }
        #endregion

        public static void Load(ContentManager content)
        {
            TestShip = content.Load<Texture2D>("Ships/TestShip");
            DebugFont = content.Load<SpriteFont>("Fonts/Debug");
            Background = content.Load<Texture2D>("starfield2");
            Bullet = CreateRectangle(10, 10, Color.White, Color.White);
        }

        public static Texture2D CreateRectangle(int width, int height, Color fillColor, Color borderColor)
        {
            Texture2D tile = new Texture2D(MainGame.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (int i = 0; i < data.Length; ++i)
            {
                if (i < width || i % width == 0 || i > width * height - width || (i + 1) % width == 0)
                {
                    data[i] = borderColor;
                }
                else
                {
                    data[i] = fillColor;
                }
            }
            tile.SetData(data);
            return tile;
        }

        public static Texture2D CreateCircle(int radius, Color borderColor)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(MainGame.Instance.GraphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = borderColor;
            }

            texture.SetData(data);
            return texture;
        }
    }
}