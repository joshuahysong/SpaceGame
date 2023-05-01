using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.UI;
using System.Collections.Generic;

namespace SpaceGame.Scenes
{
    public class MainMenuScene : IScene
    {
        private List<Button> _buttons = new();

        public void Setup()
        {
            var buttonPosition = MainGame.ScreenCenter - new Vector2(100, 50);
            var buttonTexture = Art.CreateRectangleTexture(200, 40, Color.Transparent, Color.White);
            _buttons.Add(new Button(buttonTexture, "New Game", buttonPosition, 200, 40, Color.White, StartNewGame));

            buttonPosition = MainGame.ScreenCenter - new Vector2(100, -30);
            _buttons.Add(new Button(buttonTexture, "Quit", buttonPosition, 200, 40, Color.White, Quit));
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
            spriteBatch.Draw(Art.Background, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White);
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
            MainGame.SetGameState(GameState.Space);
        }

        private void Quit()
        {
            MainGame.Instance.Exit();
        }
    }
}
