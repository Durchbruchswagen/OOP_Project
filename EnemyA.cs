using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ProjektPO
{
    class EnemyA : Enemy
    {
        public EnemyA(Level level, Vector2 position, string spriteset) : base(level, position, spriteset)
        {
        }

        public new Rectangle boundingrectangle
        {
            get
            {
                int left = (int)Math.Round(position.X - sprite.Origin.X) + localrectangle.X;
                int top = (int)Math.Round(position.Y - sprite.Origin.Y) + localrectangle.Y;
                return new Rectangle(left, top, localrectangle.Width - 15, localrectangle.Height);
            }
        }
        public override void LoadContent(string spriteSet)
        {
            spriteSet = "Sprites/" + spriteSet + "/";
            runanimation = new Animation(level.content.Load<Texture2D>(spriteSet + "run"), 0.1f, true, 4, false);
            idleanimation = new Animation(level.content.Load<Texture2D>(spriteSet + "idle"), 0.15f, true, 4, false);
            int width = (int)(idleanimation.FrameWidth * 0.5);
            int left = (idleanimation.FrameWidth - width) / 2;
            int height = (int)(idleanimation.FrameHeight * 0.7);
            int top = idleanimation.FrameHeight - height;
            localrectangle = new Rectangle(left, top, width, height);
            sprite.PlayAnimation(idleanimation);
        }
        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float posX = position.X + localrectangle.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(position.Y / Tile.Height);

            if (waitTime > 0)
            {
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                if (level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!level.player.isalive ||
                level.reachedexit ||
                waitTime > 0)
            {
                sprite.PlayAnimation(idleanimation);
            }
            else
            {
                sprite.PlayAnimation(runanimation);
            }
            SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, position, flip);
        }
    }
}
