using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ProjektPO
{
    class EnemyB : Enemy
    {
        public EnemyB(Level level, Vector2 position, string spriteset) : base(level, position, spriteset)
        {
        }
        public new Rectangle boundingrectangle
        {
            get
            {
                int left = (int)Math.Round(position.X - sprite.Origin.X) + localrectangle.X;
                int top = (int)Math.Round(position.Y - sprite.Origin.Y) + localrectangle.Y;
                return new Rectangle(left, top, localrectangle.Width, localrectangle.Height);
            }
        }
        public override void LoadContent(string spriteSet)
        {
            spriteSet = "Sprites/" + spriteSet + "/";
            runanimation = new Animation(level.content.Load<Texture2D>(spriteSet + "run"), 0.1f, true, 8, false);
            idleanimation = new Animation(level.content.Load<Texture2D>(spriteSet + "idle"), 0.15f, true, 8, false);
            int width = (int)(idleanimation.FrameWidth * 0.7);
            int left = (idleanimation.FrameWidth - width) / 2;
            int height = (int)(idleanimation.FrameHeight);
            int top = idleanimation.FrameHeight - height;
            localrectangle = new Rectangle(left, top, width, height);
            sprite.PlayAnimation(idleanimation);
        }
        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float movX;
            float movY;
            if (level.player.position.X - position.X < 0)
            {
                movX = -1 * MoveSpeed * elapsed;
                direction = FaceDirection.Left;
            }
            else
            {
                movX = MoveSpeed * elapsed;
                direction = FaceDirection.Right;
            }
            if (level.player.position.Y - position.Y < 0)
            {
                movY = -1 * MoveSpeed * elapsed;
            }
            else
            {
                movY = MoveSpeed * elapsed;
            }
            Vector2 velocity = new Vector2(movX, movY);
            position = position + velocity;
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
