using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceGame.UI;

namespace SpaceGame.Scenes
{
    public class MainMenuScene : IScene
    {

        private Button _testButton;

        public void Setup()
        {
            var buttonPosition = MainGame.ScreenCenter - new Vector2(100, 20);
            var buttonTexture = Art.CreateRectangleTexture(200, 40, Color.Transparent, Color.White);
            _testButton = new Button(buttonTexture, "New Game", buttonPosition, 200, 40, StartNewGame);
        }

        public void Update(GameTime gameTime)
        {
            _testButton.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
            spriteBatch.Draw(Art.Background, Vector2.Zero, new Rectangle(0, 0, MainGame.Viewport.Width, MainGame.Viewport.Height), Color.White);
            _testButton.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void StartNewGame()
        {
            var scene = new SpaceScene();
            scene.Setup();
            MainGame.SetScene(scene);
            MainGame.SetGameState(GameState.Space);
        }
    }
}
