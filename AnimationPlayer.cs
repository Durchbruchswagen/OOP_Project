using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ProjektPO
{
    struct AnimationPlayer
    {

        public Animation animation;
        int frameindex;
        private float time;
        public Vector2 Origin
        {
            get { return new Vector2(animation.FrameWidth / 2.0f, animation.FrameHeight); }
        }
        public void PlayAnimation(Animation anim)
        {
            if (animation == anim)
            {
                return;
            }
            if (animation == null || !animation.dontinterrupt || (animation.dontinterrupt & frameindex == (animation.framecount - 1)))
            {
                animation = anim;
                frameindex = 0;
                time = 0.0f;
            }


        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (animation == null)
            {
                throw new NotSupportedException("No animation is currently playing.");
            }
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > animation.frametime)
            {
                time -= animation.frametime;
                if (animation.islooping)
                {
                    frameindex = (frameindex + 1) % animation.framecount;
                }
                else
                {
                    frameindex = Math.Min(frameindex + 1, animation.framecount - 1);
                }
            }
            Rectangle source = new Rectangle(frameindex * animation.FrameWidth, 0, animation.FrameWidth, animation.FrameHeight);
            spriteBatch.Draw(animation.texture, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}
