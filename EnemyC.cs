using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ProjektPO
{
    class EnemyC : Enemy
    {
        Animation arrow;
        AnimationPlayer arrowsprite;
        float flytime = 0.0f;
        const float maxtime = 12.0f;
        Rectangle arrowrect
        {
            get
            {
                int left = (int)Math.Round(arrowpost.X - arrow.FrameWidth);
                int top = (int)Math.Round(arrowpost.Y - arrow.FrameHeight);
                return new Rectangle(left, top, arrow.FrameWidth, arrow.FrameHeight);
            }
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
        bool arrowspawn = false;
        Vector2 arrowpost;
        public EnemyC(Level level, Vector2 position, string spriteSet, FaceDirection dir) : base(level, position, spriteSet)
        {
            direction = dir;
        }
        public override void LoadContent(string spriteSet)
        {
            spriteSet = "Sprites/" + spriteSet + "/";
            runanimation = new Animation(level.content.Load<Texture2D>(spriteSet + "run"), 0.1f, true, 6, false);
            idleanimation = new Animation(level.content.Load<Texture2D>(spriteSet + "idle"), 0.15f, true, 6, false);
            arrow = new Animation(level.content.Load<Texture2D>(spriteSet + "fire-ball"), 0.1f, true, 3, false);
            int width = (int)(idleanimation.FrameWidth * 0.35);
            int left = (idleanimation.FrameWidth - width) / 2;
            int height = (int)(idleanimation.FrameHeight * 0.7);
            int top = idleanimation.FrameHeight - height;
            localrectangle = new Rectangle(left, top, width, height);
            sprite.PlayAnimation(idleanimation);
            arrowsprite.PlayAnimation(arrow);
        }
        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (arrowspawn)
            {
                flytime += elapsed;
                arrowpost += new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                if (flytime >= maxtime)
                {
                    arrowspawn = false;
                }
                else if (arrowrect.Intersects(level.player.boundingrectangle))
                {
                    level.player.Damaged();
                    arrowspawn = false;
                }
            }
            else
            {
                flytime = 0.0f;
                arrowspawn = true;
                arrowpost = new Vector2(boundingrectangle.X + (int)direction, boundingrectangle.Y + 20);
            }

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, position, flip);
            if (arrowspawn)
            {
                arrowsprite.Draw(gameTime, spriteBatch, arrowpost, flip);
            }
        }

    }
}
