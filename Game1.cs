using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace ProjektPO
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private const int numberOfLevels = 5;
        private KeyboardState keyboardstate;
        private bool continuepress = false;
        private Level level;
        private int levelindex = -1;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;

        }

        protected override void LoadContent()
        {
            this.Content.RootDirectory = "Content";
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadLevel();
        }
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 928;
            graphics.PreferredBackBufferHeight = 793;
            graphics.ApplyChanges();
            
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            level.Update(gameTime, keyboardstate);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, null);
            level.Draw(gameTime, spriteBatch);
            DrawHud();
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            keyboardstate = Keyboard.GetState();
            bool continuepressed = keyboardstate.IsKeyDown(Keys.Space);
            if (!continuepress && continuepressed)
            {
                if (!level.player.isalive)
                {
                    Reload();
                }
                else if (level.reachedexit)
                {
                    LoadLevel();
                }
            }
        }
        private void LoadLevel()
        {
            levelindex = (levelindex + 1) % numberOfLevels;
            if (level != null)
            {
                level.Dispose();
            }
            string levelpath = string.Format("Content/Levels/{0}.txt", levelindex);
            using (Stream filestream = TitleContainer.OpenStream(levelpath))
                level = new Level(Services, filestream, levelindex);
        }
        private void Reload()
        {
            --levelindex;
            LoadLevel();
        }

        private void DrawHud()
        {
            SpriteFont hudFont = Content.Load<SpriteFont>("Fonts/font");
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            string timeString = "HP: " + level.player.hp;
            Color hpcolor = Color.Red;
            Drawstring(hudFont, timeString, hudLocation, hpcolor);
        }
        private void Drawstring(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
