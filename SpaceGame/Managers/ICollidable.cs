using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Managers
{
    public interface ICollidable
    {
        public FactionType Faction { get; }
        public Texture2D Texture { get; }
        public Color[] TextureData { get; }
        public Vector2 Position { get; }
        public Matrix Transform { get; }
        public Rectangle BoundingRectangle { get; }
        public float Scale { get; }
        public bool IsExpired { get; }
    }
}
