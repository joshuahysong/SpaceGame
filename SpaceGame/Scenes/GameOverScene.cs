﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.UI;
using System;
using System.Collections.Generic;

namespace SpaceGame.Scenes
{
    public class GameOverScene : IScene
    {
        public string Name => SceneNames.GameOver;

        private Camera _camera;
        private List<Button> _buttons = new();
        private bool disposedValue;

        public GameOverScene()
        {
            _camera = new Camera();

            var buttonTexture = Art.CreateRectangleTexture(250, 40, Color.Black, Color.White, 2);
            _buttons.Add(new Button(buttonTexture, "New Game", TextSize.Large, new Vector2(125, 80), 250, 40, Color.White, MainGame.StartNewGame, true));
            _buttons.Add(new Button(buttonTexture, "Return to Main Menu", TextSize.Large, new Vector2(125, 20), 250, 40, Color.White, ReturnToMainMenu, true));
            _buttons.Add(new Button(buttonTexture, "Quit", TextSize.Large, new Vector2(125, -40), 250, 40, Color.White, Quit, true));
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

            var labelSize = Art.Fonts.HeaderFont.MeasureString("Game Over");
            var textX = (MainGame.Viewport.Width / 2) - (labelSize.X / 2);
            var textY = (MainGame.Viewport.Height / 2) - 200 - (labelSize.Y / 2);
            spriteBatch.DrawString(Art.Fonts.HeaderFont, "Game Over", new Vector2(textX, textY), Color.White);

            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        private void ReturnToMainMenu()
        {
            MainGame.SwitchToScene(SceneNames.MainMenu);
        }

        private void Quit()
        {
            MainGame.Instance.Exit();
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
