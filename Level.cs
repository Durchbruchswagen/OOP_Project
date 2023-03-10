using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjektPO
{
    class Level : IDisposable
    {
        private Tile[,] tiles;
        private Texture2D[] layers;
        private const int EntityLayer = 9;
        public Player player;
        private List<Enemy> enemies = new List<Enemy>();
        private Vector2 start;
        public bool reachedexit;
        public ContentManager content;
        public int width
        {
            get { return tiles.GetLength(0); }
        }
        public int height
        {
            get { return tiles.GetLength(1); }
        }
        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            content = new ContentManager(serviceProvider, "Content");
            LoadTiles(fileStream);
            layers = new Texture2D[10];
            for (int i = 0; i < layers.Length; ++i)
            {
                int segmentIndex = levelIndex;
                layers[i] = content.Load<Texture2D>("Backgrounds/Layer" + 0 + "_" + (i + 1));
            }
        }
        private void LoadTiles(Stream fileStream)
        {
            int linewidth;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                linewidth = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != linewidth)
                    {
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    }
                    line = reader.ReadLine();
                }
            }
            tiles = new Tile[linewidth, lines.Count];
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < linewidth; ++x)
                {
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }
            if (player == null)
            {
                throw new NotSupportedException("A level must have a starting point.");
            }
        }
        private Tile LoadTile(char tile, int x, int y)
        {
            switch (tile)
            {
                case '.':
                    return new Tile(null, TileCollision.Passable);
                case '-':
                    return LoadTile("tile_platform", TileCollision.Platform);
                case 'A':
                    return LoadEnemyTile(x, y, "Nightmare", FaceDirection.Left);
                case 'B':
                    return LoadEnemyTile(x, y, "Skull", FaceDirection.Left);
                case 'L':
                    return LoadEnemyTile(x, y, "Imp", FaceDirection.Left);
                case 'R':
                    return LoadEnemyTile(x, y, "Imp", FaceDirection.Right);
                case '1':
                    return LoadStartTile(x, y);
                case '#':
                    return LoadTile("tile", TileCollision.Impassable);
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tile, x, y));
            }
        }
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(content.Load<Texture2D>("Sprites/Tile/" + name), collision);
        }
        private Tile LoadStartTile(int x, int y)
        {
            if (player != null)
            {
                throw new NotSupportedException("A level may only have one starting point.");
            }
            start = RectangleHelp.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, start);

            return new Tile(null, TileCollision.Passable);
        }
        private Tile LoadEnemyTile(int x, int y, string sprite, FaceDirection dir)
        {
            Vector2 position = RectangleHelp.GetBottomCenter(GetBounds(x, y));
            if (sprite == "Nightmare")
            {
                enemies.Add(new EnemyA(this, position, sprite));
            }
            else if (sprite == "Skull")
            {
                enemies.Add(new EnemyB(this, position, sprite));
            }
            else if (sprite == "Imp")
            {
                enemies.Add(new EnemyC(this, position, sprite, dir));
            }


            return new Tile(null, 0);
        }
        public void Dispose()
        {
            content.Unload();
        }
        public TileCollision GetCollision(int x, int y)
        {
            if (x < 0 || x >= width)
                return TileCollision.Impassable;
            if (y < 0 || y >= height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }
        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if (!player.isalive)
            {
                player.ApplyPhysics(gameTime);
                player.Ondeath();
            }
            else
            {
                player.Update(gameTime, keyboardState);
                if (player.boundingrectangle.Top >= height * Tile.Height)
                {
                    player.hp = 0;
                    player.instkill();
                }
                UpdateEnemies(gameTime);
                if (player.isalive &&
                    player.isonground &&
                    !enemies.Any())
                {
                    Onexit();
                }
            }
        }
        private void UpdateEnemies(GameTime gameTime)
        {
            List<int> defeated = new List<int>();
            int ind = 0;
            int howmanyalreadydefeted = 0;
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
                if (enemy.boundingrectangle.Intersects(player.weaponrectangle) & player.isattacking)
                {
                    defeated.Add(ind);
                }
                if (enemy.boundingrectangle.Intersects(player.boundingrectangle))
                {
                    player.Damaged();
                }
                ind++;
            }
            foreach (int delete in defeated)
            {
                enemies.RemoveAt(delete - howmanyalreadydefeted);
                howmanyalreadydefeted++;
            }
        }
        private void Onexit()
        {
            reachedexit = true;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= EntityLayer; ++i)
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
            DrawTiles(spriteBatch);
            player.Draw(gameTime, spriteBatch);
            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime, spriteBatch);
            for (int i = EntityLayer + 1; i < layers.Length; ++i)
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
        }
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }
    }
}
