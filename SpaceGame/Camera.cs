using Microsoft.Xna.Framework;
using SpaceGame.Entities;

namespace SpaceGame
{
    public class Camera
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }
        public IFocusable Focus { get; set; }

        public Camera()
        {
            Scale = 1f;
        }

        public void Update()
        {
            if (Focus != null)
            {
                Position = Focus.WorldPosition;
            }
        }

        public Matrix GetTransform(Vector2 cameraCenter, float? scale = null)
        {
            var focus = Focus == null ? Vector2.Zero : Focus.WorldPosition;
            var transformScale = scale ?? Scale;

            return Matrix.CreateTranslation(new Vector3(-focus.X, -focus.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(transformScale, transformScale, 1)) *
                    Matrix.CreateTranslation(new Vector3(cameraCenter.X, cameraCenter.Y, 0));
        }
    }
}