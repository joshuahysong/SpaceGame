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
        public Matrix Transform { get; set; }
        public Matrix TransformMini { get; set; }
        public IFocusable Focus { get; set; }

        public Camera()
        {
            Scale = 1f;
        }

        public void Update()
        {
            if (Focus != null)
            {
                Transform = Matrix.CreateTranslation(new Vector3(-Focus.WorldPosition.X, -Focus.WorldPosition.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(Scale, Scale, 1)) *
                    Matrix.CreateTranslation(new Vector3(MainGame.ScreenCenter.X, MainGame.ScreenCenter.Y, 0));

                var renderCenter = new Vector2(MainGame.RenderTarget.Width / 2, MainGame.RenderTarget.Height / 2);
                TransformMini = Matrix.CreateTranslation(new Vector3(-Focus.WorldPosition.X, -Focus.WorldPosition.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(1, 1, 1)) *
                    Matrix.CreateTranslation(new Vector3(renderCenter.X, renderCenter.Y, 0));

                Origin = MainGame.ScreenCenter / Scale;
                Position = Focus.WorldPosition;
            }
        }
    }
}