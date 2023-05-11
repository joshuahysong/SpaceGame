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
        private bool _lockToScreen;
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
                ? Art.Fonts.UISmalFont
                : _textSize == TextSize.Medium
                    ? Art.Fonts.UIMediumFont
                    : Art.Fonts.UILargeFont;
            _color = _assignedColor * 0.75f;
        }

        public Button(Texture2D texture,
            string label,
            TextSize textSize,
            Vector2 position,
            int width,
            int height,
            Color color,
            Action callback,
            bool lockToScreen)
            : this(texture, label, textSize, position, width, height, color, callback)
        {
            _lockToScreen = lockToScreen;
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
            var position = _lockToScreen ? MainGame.ScreenCenter - _position : _position;
            spriteBatch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, _width, _height), _color);
            var labelSize = _font.MeasureString(_label);
            var textX = (_width / 2) - (labelSize.X / 2);
            var textY = (_height / 2) - (labelSize.Y / 2);
            spriteBatch.DrawString(_font, _label, new Vector2(position.X + textX, position.Y + textY), _color);
        }

        public bool IsMouseInButton()
        {
            var position = _lockToScreen ? MainGame.ScreenCenter - _position : _position;
            return Input.ScreenMousePosition.X < position.X + _width &&
                Input.ScreenMousePosition.X > position.X &&
                Input.ScreenMousePosition.Y < position.Y + _height &&
                Input.ScreenMousePosition.Y > position.Y;
        }
    }
}
