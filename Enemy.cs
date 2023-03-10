

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ProjektPO
{
    class Enemy
    {
        protected Animation runanimation;
        protected Animation idleanimation;
        protected AnimationPlayer sprite;
        protected Level level;
        protected Vector2 position;
        protected Rectangle localrectangle;
        public Rectangle boundingrectangle
        {
            get
            {
                int left = (int)Math.Round(position.X - sprite.Origin.X) + localrectangle.X;
                int top = (int)Math.Round(position.Y - sprite.Origin.Y) + localrectangle.Y;
                return new Rectangle(left, top, localrectangle.Width, localrectangle.Height);
            }
        }
        protected FaceDirection direction = FaceDirection.Left;
        protected float waitTime;
        protected const float MaxWaitTime = 0.5f;
        protected const float MoveSpeed = 64.0f;
        public Enemy(Level level, Vector2 position, string spriteset)
        {
            this.level = level;
            this.position = position;
            LoadContent(spriteset);
        }
        public virtual void LoadContent(string spriteSet)
        {

        }
        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
    }
}
