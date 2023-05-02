using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
using SpaceGame.UI;
using System.Collections.Generic;

namespace SpaceGame.Scenes
{
    public class GameOverScene : IScene
    {
        private List<Button> _buttons = new();

        public void Setup()
        {
            var buttonPosition = MainGame.ScreenCenter - new Vector2(125, 80);
            var buttonTexture = Art.CreateRectangleTexture(250, 40, Color.Transparent, Color.White);
            _buttons.Add(new Button(buttonTexture, "New Game", TextSize.Medium, buttonPosition, 250, 40, Color.White, StartNewGame));

            buttonPosition = MainGame.ScreenCenter - new Vector2(125, 20);
            _buttons.Add(new Button(buttonTexture, "Return to Main Menu", TextSize.Medium, buttonPosition, 250, 40, Color.White, ReturnToMainMenu));

            buttonPosition = MainGame.ScreenCenter - new Vector2(125, -40);
            _buttons.Add(new Button(buttonTexture, "Quit", TextSize.Medium, buttonPosition, 250, 40, Color.White, Quit));
        }

        public void Update(GameTime gameTime)
        {
            foreach (var button in _buttons)
            {
                button.Update();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.Backgrounds.BlueNebula1, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.75f);

            var labelSize = Art.HeaderFont.MeasureString("Game Over");
            var textX = (MainGame.Viewport.Width / 2) - (labelSize.X / 2);
            var textY = (MainGame.Viewport.Height / 2) - 200 - (labelSize.Y / 2);
            spriteBatch.DrawString(Art.HeaderFont, "Game Over", new Vector2(textX, textY), Color.White);

            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        private void StartNewGame()
        {
            var scene = new SpaceScene();
            scene.Setup();
            MainGame.SetScene(scene);
        }

        private void ReturnToMainMenu()
        {
            EntityManager.Initialize();
            CollisionManager.Initialize();
            ParticleEffectsManager.Initialize();

            var scene = new MainMenuScene();
            scene.Setup();
            MainGame.SetScene(scene);
        }

        private void Quit()
        {
            MainGame.Instance.Exit();
        }
    }
}
