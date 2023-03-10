using Microsoft.Xna.Framework.Graphics;

namespace ProjektPO
{
    class Animation
    {
        public Texture2D texture;
        public float frametime;
        public bool islooping;
        public bool dontinterrupt;
        public int framecount;

        public int FrameWidth
        {
            get { return (texture.Width / framecount); }
        }
        public int FrameHeight
        {
            get { return texture.Height; }
        }
        public Animation(Texture2D texture, float frameTime, bool isLooping, int FrameCount, bool doint)
        {
            this.texture = texture;
            this.frametime = frameTime;
            this.islooping = isLooping;
            framecount = FrameCount;
            dontinterrupt = doint;
        }
    }
}
