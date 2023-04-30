using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Ships.Parts
{
    public class Thruster1
    {
        private Texture2D _texture;
        private Vector2 _position;
        private Vector2 _rotation;
        private Vector2 _origin;
        private float _scale;

        private Matrix _localTransform =>
            Matrix.CreateTranslation(0, 0, 0f) *
            Matrix.CreateScale(1f, 1f, 1f) *
            Matrix.CreateRotationZ(_rotation.ToAngle()) *
            Matrix.CreateTranslation(_position.X, _position.Y, 0f);

        public Thruster1(Texture2D texture, Vector2 position, Vector2 rotation, float scale)
        {
            _texture = texture;
            _position = position;
            _rotation = rotation;
            _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
            _scale = scale;
        }

        public void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            Matrix globalTransform = _localTransform * parentTransform;
            MathUtilities.DecomposeMatrix(ref globalTransform, out Vector2 position, out float rotation, out Vector2 scale);

            spriteBatch.Draw(_texture, position, null, Color.White, rotation, _origin, _scale, SpriteEffects.None, 1);
        }
    }
}
