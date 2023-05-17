using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SpaceGame.Common;
using SpaceGame.Managers;
using System;
using System.Collections.Generic;

namespace SpaceGame.Scenes.Components
{
    public class Planet : IDockable
    {
        public Guid Id { get; private set; }
        public FactionType Faction { get; private set; }
        public Vector2 Position { get; private set; }
        public bool IsExpired { get; private set; }
        public Texture2D Texture { get; private set; }
        public Color[] TextureData { get; private set; }
        public float Scale { get; private set; }
        public List<string> Description { get; private set; }
        public string CurrentSolarSystemName { get; set; }

        public Matrix Transform => Matrix.CreateTranslation(new Vector3(-_origin, 0.0f) * Scale)
            * Matrix.CreateRotationZ(0f)
            * Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        public Rectangle BoundingRectangle => CollisionManager.CalculateBoundingRectangle(_rectangle, Transform);

        private Rectangle _rectangle;
        private Vector2 _origin;
        private Texture2D _boundingBoxTexture;

        public Planet(
            FactionType faction,
            Vector2 position,
            Texture2D texture,
            List<string> description,
            float scale = 1f)
        {
            Id = Guid.NewGuid();
            Faction = faction;
            Position = position;
            Texture = texture;
            Scale = scale;
            TextureData = Art.GetScaledTextureData(Texture, Scale);
            Description = description;
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, (int)Math.Floor(Texture.Width * Scale), (int)Math.Floor(Texture.Height * Scale));
            _boundingBoxTexture = Art.CreateRectangleTexture(BoundingRectangle.Width, BoundingRectangle.Height, Color.Transparent, Color.White);
            CollisionManager.Add(this);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform, bool drawMinimized = false)
        {
            if (drawMinimized)
            {
                var color = Faction switch
                {
                    FactionType.None => Color.LightGray,
                    FactionType.Player => Color.Green,
                    FactionType.Enemy => Color.Red,
                    _ => Color.White,
                };
                spriteBatch.DrawCircle(Position, Texture.Width / 2, 32, color, 30);
                return;
            }

            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, _origin, Scale, 0, 0);

            if (MainGame.IsDebugging)
            {
                spriteBatch.Draw(_boundingBoxTexture, BoundingRectangle, Color.White);
            }
        }
    }
}
