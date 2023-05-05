using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
using System;

namespace SpaceGame.SolarSystems.Models
{
    public class Planet : ICollidable
    {
        public Guid Id { get; set; }
        public FactionType Faction { get; set; }
        public Vector2 Position { get; set; }
        public bool IsExpired { get; set; }
        public Texture2D Texture { get; set; }
        public Color[] TextureData { get; set; }
        public float Scale { get; set; }

        public Matrix Transform => Matrix.CreateTranslation(new Vector3(-_origin, 0.0f) * Scale)
            * Matrix.CreateRotationZ(0f)
            * Matrix.CreateTranslation(new Vector3(Position, 0.0f));

        public Rectangle BoundingRectangle => CollisionManager.CalculateBoundingRectangle(_rectangle, Transform);

        private readonly Rectangle _rectangle;
        private readonly Vector2 _origin;
        private Texture2D _boundingBoxTexture;

        public Planet(
            FactionType faction,
            Vector2 position,
            Texture2D texture,
            float scale = 1f)
        {
            Id = Guid.NewGuid();
            Faction = faction;
            Position = position;
            Texture = texture;
            Scale = scale;
            TextureData = Art.GetScaledTextureData(Texture, Scale);
            _origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            _rectangle = new Rectangle(0, 0, (int)Math.Floor(Texture.Width * Scale), (int)Math.Floor(Texture.Height * Scale));
            _boundingBoxTexture = Art.CreateRectangleTexture(BoundingRectangle.Width, BoundingRectangle.Height, Color.Transparent, Color.White);
            CollisionManager.Add(this);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, _origin, Scale, 0, 0);

            if (MainGame.IsDebugging)
            {
                spriteBatch.Draw(_boundingBoxTexture, BoundingRectangle, Color.White);
            }
        }
    }
}
