using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceGame.UI
{
    public class Button
    {
        public Texture2D Texture { get; set; }
        public string Label { get; set; }
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Action Callback { get; set; }

        public Button(Texture2D texture,
            string label,
            Vector2 position,
            int width,
            int height,
            Action callback)
        {
            Texture = texture;
            Label = label;
            Position = position;
            Width = width;
            Height = height;
            Callback = callback;
        }

        public void Update()
        {
            if (Input.WasLeftMouseButtonClicked())
            {
                Console.WriteLine("Left Button Clicked");
                if (IsMouseInButton())
                {
                    Callback.Invoke();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
        }

        public bool IsMouseInButton()
        {
            return Input.ScreenMousePosition.X < Position.X + Width &&
                Input.ScreenMousePosition.X > Position.X &&
                Input.ScreenMousePosition.Y < Position.Y + Height &&
                Input.ScreenMousePosition.Y > Position.Y;
        }
    }
}
