using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Managers
{
    public interface ICollidable
    {
        public Texture2D Texture { get; }
        public Color[] TextureData { get; }
        public Matrix Transform { get; }
        public Rectangle BoundingRectangle { get; }
    }
}
