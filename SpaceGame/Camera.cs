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
        public IFocusable Focus { get; set; }

        private int _previousScrollValue;

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

                Origin = MainGame.ScreenCenter / Scale;
                Position = Focus.WorldPosition;
            }
        }

        public void HandleInput()
        {
            var zoomStep = 0.1f;
            var maximumZoom = 3f;
            var minimumZoom = 0.3f;

            if (Input.MouseScrollWheelValue < _previousScrollValue)
            {
                Scale -= zoomStep;
                if (Scale < minimumZoom)
                {
                    Scale = minimumZoom;
                }
            }
            else if (Input.MouseScrollWheelValue > _previousScrollValue)
            {
                Scale += zoomStep;
                if (Scale > maximumZoom)
                {
                    Scale = maximumZoom;
                }
            }
            _previousScrollValue = Input.MouseScrollWheelValue;
        }
    }
}