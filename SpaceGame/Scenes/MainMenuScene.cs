﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.UI;
using System;
using System.Collections.Generic;

namespace SpaceGame.Scenes
{
    public class MainMenuScene : IScene
    {
        public string Name => SceneNames.MainMenu;

        private Camera _camera;
        private Texture2D _background;
        private List<Button> _buttons = new();
        private bool disposedValue;

        public MainMenuScene()
        {
            _camera = new Camera();

            var buttonTexture = Art.CreateRectangleTexture(200, 40, Color.Black, Color.White, 2);
            _buttons.Add(new Button(buttonTexture, "New Game", TextSize.Large, new Vector2(100, 50), 200, 40, Color.White, MainGame.StartNewGame, true));
            _buttons.Add(new Button(buttonTexture, "Quit", TextSize.Large, new Vector2(100, -30), 200, 40, Color.White, Quit, true));
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
                var index = random.Next(0, Art.Backgrounds.All.Count);
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
