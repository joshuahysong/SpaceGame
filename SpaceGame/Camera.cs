using Microsoft.Xna.Framework;
using SpaceGame.Entities;

namespace SpaceGame
{
    public class Camera
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
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
                Origin = MainGame.ScreenCenter / Scale;
                Position = Focus.WorldPosition;
            }
        }

        public Matrix GetTransform(Vector2 cameraCenter)
        {
            var focus = Focus == null ? Vector2.Zero : Focus.WorldPosition;

            return Matrix.CreateTranslation(new Vector3(-focus.X, -focus.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(Scale, Scale, 1)) *
                    Matrix.CreateTranslation(new Vector3(cameraCenter.X, cameraCenter.Y, 0));
        }
    }
}