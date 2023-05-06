using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceGame.UI
{
    public class Button
    {
        private Texture2D _texture;
        private string _label;
        private Vector2 _position;
        private int _width;
        private int _height;
        private Action _callback;
        private TextSize _textSize;
        private Color _assignedColor;
        private SpriteFont _font;
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
            _texture = texture;
            _label = label;
            _position = position;
            _width = width;
            _height = height;
            _callback = callback;
            _textSize = textSize;
            _assignedColor = color;
            _font = _textSize == TextSize.Small
                ? Art.UISmalFont
                : _textSize == TextSize.Medium
                    ? Art.UIMediumFont
                    : Art.UILargeFont;
            _color = _assignedColor * 0.75f;
        }

        public void Update()
        {
            if (IsMouseInButton())
            {
                _color = _assignedColor;
                if (Input.WasLeftMouseButtonClicked())
                {
                    _callback.Invoke();
                }
            }
            else
            {
                _color = _assignedColor * 0.75f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, new Rectangle((int)_position.X, (int)_position.Y, _width, _height), _color);
            var labelSize = _font.MeasureString(_label);
            var textX = (_width / 2) - (labelSize.X / 2);
            var textY = (_height / 2) - (labelSize.Y / 2);
            spriteBatch.DrawString(_font, _label, new Vector2(_position.X + textX, _position.Y + textY), _color);
        }

        public bool IsMouseInButton()
        {
            return Input.ScreenMousePosition.X < _position.X + _width &&
                Input.ScreenMousePosition.X > _position.X &&
                Input.ScreenMousePosition.Y < _position.Y + _height &&
                Input.ScreenMousePosition.Y > _position.Y;
        }
    }
}
