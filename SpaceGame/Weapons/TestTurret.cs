using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Projectiles;

namespace SpaceGame.Weapons
{
    public class TestTurret : WeaponBase
    {
        private Texture2D _texture;
        private Vector2 _origin;

        public TestTurret(Vector2 position)
            : base(position,
                projectileSourcePosition: new Vector2(15, 0),
                cooldown: 0.2f,
                speed: 1000f,
                accuracy: 99f,
                projectileType: typeof(GreenBullet))
        {
            _texture = Art.Parts.Turret1;
            _origin = new Vector2(10.5f, _texture.Height / 2);
        }

        public override void Update(GameTime gameTime, Matrix parentTransform)
        {
            Matrix globalTransform = _localTransform * parentTransform;
            MathUtilities.DecomposeMatrix(ref globalTransform, out Vector2 position, out float rotation, out Vector2 scale);

            var targetPosition = Input.WorldMousePosition;
            Vector2 direction = Vector2.Normalize(targetPosition - position);

            MathUtilities.DecomposeMatrix(ref parentTransform, out Vector2 parentPosition, out float parentRotation, out Vector2 parentScale);

            _rotation = direction.ToAngle() - parentRotation;

            base.Update(gameTime, parentTransform);
        }

        public override void Draw(SpriteBatch spriteBatch, Matrix parentTransform)
        {
            Matrix globalTransform = _localTransform * parentTransform;
            MathUtilities.DecomposeMatrix(ref globalTransform, out Vector2 position, out float rotation, out Vector2 scale);
            spriteBatch.Draw(_texture, position, null, Color.White, rotation, _origin, 0.5f, SpriteEffects.None, 1);

            base.Draw(spriteBatch, parentTransform);
        }
    }
}
