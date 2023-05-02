using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.Managers;
using SpaceGame.UI;
using System.Collections.Generic;

namespace SpaceGame.Scenes
{
    public class PauseMenuScene : IScene
    {
        private List<Button> _buttons = new();

        public void Setup()
        {
            var buttonPosition = MainGame.ScreenCenter - new Vector2(125, 80);
            var buttonTexture = Art.CreateRectangleTexture(250, 40, Color.Transparent, Color.White);
            _buttons.Add(new Button(buttonTexture, "Resume", buttonPosition, 250, 40, Color.White, Resume));

            buttonPosition = MainGame.ScreenCenter - new Vector2(125, 20);
            _buttons.Add(new Button(buttonTexture, "Return to Main Menu", buttonPosition, 250, 40, Color.White, ReturnToMainMenu));

            buttonPosition = MainGame.ScreenCenter - new Vector2(125, -40);
            _buttons.Add(new Button(buttonTexture, "Quit", buttonPosition, 250, 40, Color.White, Quit));
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
            spriteBatch.Draw(Art.Backgrounds.BlueNebula1, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White);
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
