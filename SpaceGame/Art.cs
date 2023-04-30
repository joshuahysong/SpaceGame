using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceGame
{
    public static class Art
    {
        #region Backgrounds
        public static Texture2D Background { get; set; }
        #endregion

        #region Ships
        public static Texture2D TestShip1 { get; set; }
        public static Texture2D TestShip2 { get; set; }
        #endregion

        #region Projectiles
        public static Texture2D Bullet { get; set; }
        public static Texture2D GreenBullet { get; set; }
        public static Texture2D OrangeLaser { get; set; }
        #endregion

        #region Effects
        public static Texture2D Thruster1 { get; set; }
        #endregion

        #region Fonts
        public static SpriteFont DebugFont { get; private set; }
        public static SpriteFont UIFont { get; private set; }
        #endregion

        public static Texture2D Pixel { get; set; }

        public static void Load(ContentManager content)
        {
            Background = content.Load<Texture2D>("starfield2");

            TestShip1 = content.Load<Texture2D>("Ships/TestShip1");
            TestShip2 = content.Load<Texture2D>("Ships/TestShip2");

            Bullet = CreateRectangle(5, 5, Color.White, Color.White);
            GreenBullet = content.Load<Texture2D>("Projectiles/green_bullet");
            OrangeLaser = content.Load<Texture2D>("Projectiles/orange_laser");

            Thruster1 = content.Load<Texture2D>("Effects/thruster-1");

            DebugFont = content.Load<SpriteFont>("Fonts/Debug");

            Pixel = new Texture2D(MainGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new[] { Color.White });
        }

        public static Texture2D CreateRectangle(int width, int height, Color fillColor, Color borderColor)
        {
            Texture2D texture = new Texture2D(MainGame.Instance.GraphicsDevice, width, height);
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
            texture.SetData(data);
            return texture;
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

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(Pixel, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 position, int radius, Color borderColor)
        {
            var circle = CreateCircle(radius, borderColor);
            var circlePosition = new Vector2(position.X - radius, position.Y - radius);
            spriteBatch.Draw(circle, circlePosition, null, borderColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, float heading, Color borderColor)
        {
            var rectangle = CreateRectangle((int)Math.Ceiling(size.X), (int)Math.Ceiling(size.Y), Color.Transparent, borderColor);
            spriteBatch.Draw(rectangle, position, null, borderColor, heading, size / 2f, 1f, SpriteEffects.None, 0);
        }

        public static Color[] GetScaledTextureData(Texture2D texture, float scale)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            if (scale == ScaleType.Full)
                return data;

            // TODO Add support for other scales
            if (scale != ScaleType.Half)
                throw new ArgumentException("Must be a valid scale", nameof(scale));

            float modulus = 2;
            var columnsToKeep = Enumerable.Range(0, texture.Width - 1).Where((x, i) => i % modulus == 0).ToList();
            var rowsToKeep = Enumerable.Range(0, texture.Height - 1).Where((x, i) => i % modulus == 0).ToList();
            var numberOfRows = texture.Height;
            var bothRemoved = new List<Color>();
            var keptRows = new List<Color[]>();
            rowsToKeep.ToList().ForEach(x =>
            {
                keptRows.Add(data.Where((z, i) => i >= x * texture.Width && i < (x + 1) * texture.Width).ToArray());
            });

            keptRows.ForEach(x =>
            {
                bothRemoved.AddRange(x.Where((y, i) => columnsToKeep.Contains(i)));
            });

            return bothRemoved.ToArray();
        }
    }
}