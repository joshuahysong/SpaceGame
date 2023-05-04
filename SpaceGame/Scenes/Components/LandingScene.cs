using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;

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

            var descriptionText = new List<string>();
            descriptionText.Add($"The sun beats down on the empty spaceport, its harsh rays reflecting off the sand dunes." +
                $"The only sound is the wind whistling through the abandoned buildings." +
                $"The spaceport is a reminder of a time when this planet was thriving, but now it is nothing more than a ghost town.");
            descriptionText.Add($"The once-busy spaceport is now deserted. The docking bays are empty, the hangars are closed, and the control tower is silent." +
                $"The only signs of life are the occasional sand lizards that scuttle across the tarmac.");
            descriptionText.Add($"The spaceport is a relic of a bygone era." +
                $"It was once a hub of commerce and trade, but now it is nothing more than a monument to a lost civilization. The buildings are crumbling," +
                $"the ships are rusting, and the sand is slowly encroaching on the tarmac.");
            descriptionText.Add($"The spaceport is a reminder of the fragility of life." +
                $"It is a reminder that even the most prosperous civilizations can be brought to ruin. It is a reminder that nothing lasts forever.");

            var textX = MainGame.Viewport.Width / 2 - _testTexture.Width / 2 + 10;
            var textY = MainGame.Viewport.Height / 2 - _testTexture.Height / 2 + 10;
            foreach (var text in descriptionText)
            {
                var splitText = text.WrapText(Art.UISmalFont, 620);
                spriteBatch.DrawString(Art.UISmalFont, splitText, new Vector2(textX, textY), Color.White);
                textY += (splitText.Count(x => x == '\n') + 2) * ((int)Art.UISmalFont.MeasureString(text).Y);
            }

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
