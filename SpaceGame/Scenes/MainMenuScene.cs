using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.UI;
using System;
using System.Collections.Generic;

namespace SpaceGame.Scenes
{
    public class MainMenuScene : IScene
    {
        private Camera _camera;
        private Texture2D _background;
        private List<Button> _buttons = new();

        public MainMenuScene()
        {
            _camera = new Camera();

            var buttonPosition = MainGame.ScreenCenter - new Vector2(100, 50);
            var buttonTexture = Art.CreateRectangleTexture(200, 40, Color.Black, Color.White, 2);
            _buttons.Add(new Button(buttonTexture, "New Game", TextSize.Large, buttonPosition, 200, 40, Color.White, StartNewGame));

            buttonPosition = MainGame.ScreenCenter - new Vector2(100, -30);
            _buttons.Add(new Button(buttonTexture, "Quit", TextSize.Large, buttonPosition, 200, 40, Color.White, Quit));
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
            if (_background == null)
            {
                var random = new Random();
                var index = random.Next(0, Art.Backgrounds.All.Count - 1);
                _background = Art.Backgrounds.All.ToArray()[index];
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(_background, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White * 0.75f);
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        private void StartNewGame()
        {
            MainGame.SetScene(new SpaceScene());
        }

        private void Quit()
        {
            MainGame.Instance.Exit();
        }
    }
}
