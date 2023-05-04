﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.UI;
using System;

namespace SpaceGame.Scenes.Components
{
    public class LandingScene
    {
        public bool IsExiting { get; private set; }

        private Texture2D _testTexture;
        private Button _button1;
        private Button _button2;
        private Button _exitButton;

        public LandingScene()
        {
            _testTexture = Art.CreateRectangleTexture(640, 300, Color.Black, Color.White);

            var buttonWidth = 200;
            var buttonHeight = 40;
            var buttonTexture = Art.CreateRectangleTexture(250, 40, Color.Black, Color.White);
            var button1Location = MainGame.ScreenCenter + new Vector2(-(_testTexture.Width / 2), _testTexture.Height / 2 + 20);
            var button2Location = button1Location + new Vector2(buttonWidth + 20, 0);
            var button3Location = button2Location + new Vector2(buttonWidth + 20, 0);
            _button1 = new Button(buttonTexture, "Button 1", TextSize.Large, button1Location, buttonWidth, buttonHeight, Color.White, Exit);
            _button2 = new Button(buttonTexture, "Button 2", TextSize.Large, button2Location, buttonWidth, buttonHeight, Color.White, Exit);
            _exitButton = new Button(buttonTexture, "Launch", TextSize.Large, button3Location, buttonWidth, buttonHeight, Color.White, Exit);
        }

        public void Update(GameTime gameTime)
        {
            _button1.Update();
            _button2.Update();
            _exitButton.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(_testTexture, MainGame.ScreenCenter - new Vector2(_testTexture.Width / 2, _testTexture.Height / 2), Color.White);

            var text = $"The planet is a barren waste land. Mainly because the developer is lazy...".WrapText(Art.UIMediumFont, 600);
            var textX = MainGame.Viewport.Width / 2 - _testTexture.Width / 2 + 10;
            var textY = MainGame.Viewport.Height / 2 - _testTexture.Height /2 + 10;
            spriteBatch.DrawString(Art.UIMediumFont, text, new Vector2(textX, textY), Color.White);

            _button1.Draw(spriteBatch);
            _button2.Draw(spriteBatch);
            _exitButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void Exit()
        {
            IsExiting = true;
        }
    }
}
