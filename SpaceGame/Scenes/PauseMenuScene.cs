using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.UI;
using System.Collections.Generic;

namespace SpaceGame.Scenes
{
    public class PauseMenuScene : IScene
    {
        private Camera _camera;
        private List<Button> _buttons = new();

        public PauseMenuScene()
        {
            _camera = new Camera();

            var buttonPosition = MainGame.ScreenCenter - new Vector2(125, 80);
            var buttonTexture = Art.CreateRectangleTexture(250, 40, Color.Black, Color.White, 2);
            _buttons.Add(new Button(buttonTexture, "Resume", TextSize.Large, buttonPosition, 250, 40, Color.White, Resume));

            buttonPosition = MainGame.ScreenCenter - new Vector2(125, 20);
            _buttons.Add(new Button(buttonTexture, "Return to Main Menu", TextSize.Large, buttonPosition, 250, 40, Color.White, ReturnToMainMenu));

            buttonPosition = MainGame.ScreenCenter - new Vector2(125, -40);
            _buttons.Add(new Button(buttonTexture, "Quit", TextSize.Large, buttonPosition, 250, 40, Color.White, Quit));
        }

        public void Update(GameTime gameTime)
        {
            if (MainGame.Instance.IsActive)
            {
                Input.Update();
            }
            _camera.Update();

            foreach (var button in _buttons)
            {
                button.Update();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.Backgrounds.BlueNebula1, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.75f);
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        private void Resume()
        {
            MainGame.SwitchToPreviousScene();
        }

        private void ReturnToMainMenu()
        {
            MainGame.SetScene(new MainMenuScene());
        }

        private void Quit()
        {
            MainGame.Instance.Exit();
        }
    }
}
