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

        private TextSize _textSize;
        private Color _assignedColor;
        private Color _color;

        public Button(Texture2D texture,
            string label,
            TextSize textSize,
            Vector2 position,
            int width,
            int height,
            Color color,
            Action callback)
        {
            Texture = texture;
            Label = label;
            Position = position;
            Width = width;
            Height = height;
            Callback = callback;
            _textSize = textSize;
            _assignedColor = color;
            _color = _assignedColor * 0.75f;
        }

        public void Update()
        {
            if (IsMouseInButton())
            {
                _color = _assignedColor;
                if (Input.WasLeftMouseButtonClicked())
                {
                    Callback.Invoke();
                }
            }
            else
            {
                _color = _assignedColor * 0.75f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), _color);
            var font = _textSize == TextSize.Small ? Art.UISmalFont : Art.UIMediumFont;
            var labelSize = font.MeasureString(Label);
            var textX = (Width / 2) - (labelSize.X / 2);
            var textY = (Height / 2) - (labelSize.Y / 2);
            spriteBatch.DrawString(font, Label, new Vector2(Position.X + textX, Position.Y + textY), _color);
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
