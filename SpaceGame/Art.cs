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
        public static class Backgrounds
        {
            public static Texture2D BlueNebula1 { get; set; }
            public static Texture2D BlueNebula2 { get; set; }
            public static Texture2D BlueNebula3 { get; set; }
            public static Texture2D BlueNebula4 { get; set; }
            public static Texture2D BlueNebula5 { get; set; }
            public static Texture2D BlueNebula6 { get; set; }
            public static Texture2D BlueNebula7 { get; set; }
            public static Texture2D GreenNebula1 { get; set; }
            public static Texture2D GreenNebula2 { get; set; }
            public static Texture2D GreenNebula3 { get; set; }
            public static Texture2D GreenNebula4 { get; set; }
            public static Texture2D PurpleNebula1 { get; set; }
            public static Texture2D PurpleNebula2 { get; set; }
            public static Texture2D PurpleNebula3 { get; set; }
            public static Texture2D PurpleNebula4 { get; set; }
            public static Texture2D PurpleNebula5 { get; set; }
            public static Texture2D Starfield1 { get; set; }
            public static List<Texture2D> All => new()
            {
                BlueNebula1,
                BlueNebula2,
                BlueNebula3,
                BlueNebula4,
                BlueNebula5,
                BlueNebula6,
                BlueNebula7,
                GreenNebula1,
                GreenNebula2,
                GreenNebula3,
                GreenNebula4,
                PurpleNebula1,
                PurpleNebula2,
                PurpleNebula3,
                PurpleNebula4,
                PurpleNebula5
            };
        }
        #endregion

        #region Planets
        public static class Planets
        {
            public static Texture2D Cloudy4 { get; set; }
            public static Texture2D RedPlanet { get; set; }
        }
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
        public static class Fonts
        {
            public static SpriteFont DebugFont { get; set; }
            public static SpriteFont HeaderFont { get; set; }
            public static SpriteFont UISmalFont { get; set; }
            public static SpriteFont UIMediumFont { get; set; }
            public static SpriteFont UILargeFont { get; set; }
        }
        #endregion

        #region Map
        public static class Map
        {
            public static Texture2D SolarSystem { get; set; }
        }
        #endregion

        #region Misc
        public static class Misc
        {
            public static Texture2D Pixel { get; set; }
            public static Texture2D CircleFilled { get; set; }
            public static Texture2D CircleOutline { get; set; }
        }
        #endregion

        public static void Load(ContentManager content)
        {
            Backgrounds.BlueNebula1 = content.Load<Texture2D>("Backgrounds/blue-nebula-1");
            Backgrounds.BlueNebula2 = content.Load<Texture2D>("Backgrounds/blue-nebula-2");
            Backgrounds.BlueNebula3 = content.Load<Texture2D>("Backgrounds/blue-nebula-3");
            Backgrounds.BlueNebula4 = content.Load<Texture2D>("Backgrounds/blue-nebula-4");
            Backgrounds.BlueNebula5 = content.Load<Texture2D>("Backgrounds/blue-nebula-5");
            Backgrounds.BlueNebula6 = content.Load<Texture2D>("Backgrounds/blue-nebula-6");
            Backgrounds.BlueNebula7 = content.Load<Texture2D>("Backgrounds/blue-nebula-7");
            Backgrounds.GreenNebula1 = content.Load<Texture2D>("Backgrounds/green-nebula-1");
            Backgrounds.GreenNebula2 = content.Load<Texture2D>("Backgrounds/green-nebula-2");
            Backgrounds.GreenNebula3 = content.Load<Texture2D>("Backgrounds/green-nebula-3");
            Backgrounds.GreenNebula4 = content.Load<Texture2D>("Backgrounds/green-nebula-4");
            Backgrounds.PurpleNebula1 = content.Load<Texture2D>("Backgrounds/purple-nebula-1");
            Backgrounds.PurpleNebula2 = content.Load<Texture2D>("Backgrounds/purple-nebula-2");
            Backgrounds.PurpleNebula3 = content.Load<Texture2D>("Backgrounds/purple-nebula-3");
            Backgrounds.PurpleNebula4 = content.Load<Texture2D>("Backgrounds/purple-nebula-4");
            Backgrounds.PurpleNebula5 = content.Load<Texture2D>("Backgrounds/purple-nebula-5");
            Backgrounds.Starfield1 = content.Load<Texture2D>("Backgrounds/starfield-1");

            Planets.Cloudy4 = content.Load<Texture2D>("Planets/cloudy-4");
            Planets.RedPlanet = content.Load<Texture2D>("Planets/red-planet");

            TestShip1 = content.Load<Texture2D>("Ships/TestShip1");
            TestShip2 = content.Load<Texture2D>("Ships/TestShip2");

            Bullet = CreateRectangleTexture(5, 5, Color.White, Color.White);
            GreenBullet = content.Load<Texture2D>("Projectiles/green_bullet");
            OrangeLaser = content.Load<Texture2D>("Projectiles/orange_laser");

            Thruster1 = content.Load<Texture2D>("Effects/thruster-1");

            Fonts.DebugFont = content.Load<SpriteFont>("Fonts/Debug");
            Fonts.HeaderFont = content.Load<SpriteFont>("Fonts/Header");
            Fonts.UISmalFont = content.Load<SpriteFont>("Fonts/UI_Small");
            Fonts.UIMediumFont = content.Load<SpriteFont>("Fonts/UI_Medium");
            Fonts.UILargeFont = content.Load<SpriteFont>("Fonts/UI_Large");

            Map.SolarSystem = content.Load<Texture2D>("Map/solar-system");

            Misc.CircleFilled = content.Load<Texture2D>("Misc/circle-filled");
            Misc.CircleOutline = content.Load<Texture2D>("Misc/circle-outline");
            Misc.Pixel = new Texture2D(MainGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Misc.Pixel.SetData(new[] { Color.White });
        }

        public static Texture2D CreateRectangleTexture(int width, int height, Color fillColor, Color borderColor, int thickness = 1)
        {
            Texture2D texture = new Texture2D(MainGame.Instance.GraphicsDevice, width, height);
            Color[] data = new Color[width * height];
            for (var i = 0; i < data.Length; ++i)
            {
                if (i < width * thickness ||
                    i > width * height - width * thickness)
                {
                    data[i] = borderColor;
                }
                else
                {
                    data[i] = fillColor;
                }
                for (var x = 0; x < thickness; x++)
                {
                    if (i % width - x == 0 ||
                        (i + 1 + x) % width == 0)
                    {
                        data[i] = borderColor;
                    }
                }
            }
            texture.SetData(data);
            return texture;
        }

        public static Texture2D CreateCircleTexture(int radius, Color borderColor)
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
            spriteBatch.Draw(Misc.Pixel, point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 position, int radius, Color borderColor)
        {
            var circle = CreateCircleTexture(radius, borderColor);
            var circlePosition = new Vector2(position.X - radius, position.Y - radius);
            spriteBatch.Draw(circle, circlePosition, null, borderColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 position, Vector2 size, float heading, Color borderColor)
        {
            var rectangle = CreateRectangleTexture((int)Math.Ceiling(size.X), (int)Math.Ceiling(size.Y), Color.Transparent, borderColor);
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