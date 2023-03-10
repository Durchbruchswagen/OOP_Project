using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ProjektPO
{
    class Player
    {
        private Animation idleanimation;
        private Animation runanimation;
        private Animation jumpanimation;
        private Animation dieanimation;
        private Animation attack;
        private AnimationPlayer sprite;
        public const int maxhp = 6;
        public int hp;
        FaceDirection dir;
        public Level level;
        public bool isalive;
        private bool inv = false;
        private const float maxiframe = 1.0f;
        private float iframe = 0;
        public Vector2 position;
        private float previousbottom;
        public Vector2 velocity;
        public bool isonground;
        private float movement;
        private bool isjumping;
        private bool wasjumping;
        private float jumptime;
        public bool isattacking = false;
        private bool wantstoattack;
        private SpriteEffects flip = SpriteEffects.None;
        private Rectangle localrectangle
        {
            get
            {
                int width = (int)(sprite.animation.FrameWidth * 0.4);
                int left = (sprite.animation.FrameWidth - width) / 2;
                int height = (int)(sprite.animation.FrameHeight * 0.8);
                int top = sprite.animation.FrameHeight - height;
                return new Rectangle(left, top, width, height);
            }
        }
        public Rectangle boundingrectangle
        {
            get
            {
                int left = (int)Math.Round(position.X - sprite.Origin.X) + localrectangle.X;
                int top = (int)Math.Round(position.Y - sprite.Origin.Y) + localrectangle.Y;
                return new Rectangle(left, top, localrectangle.Width, localrectangle.Height);
            }
        }
        public Rectangle weaponrectangle
        {
            get
            {
                int left = boundingrectangle.X + boundingrectangle.Width / 2;
                int top = boundingrectangle.Y + (boundingrectangle.Height / 2) - 3;
                if (dir == FaceDirection.Left)
                {
                    return new Rectangle(left - 58, top, 58, 4);
                }
                return new Rectangle(left, top, (int)dir * 58, 4);
            }
        }
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;
        public Player(Level level, Vector2 position)
        {
            this.level = level;
            LoadContent();
            Reset(position);
            hp = maxhp;
            dir = FaceDirection.Right;
        }
        public void LoadContent()
        {
            idleanimation = new Animation(level.content.Load<Texture2D>("Sprites/Player/idle"), 0.1f, true, 4, false);
            runanimation = new Animation(level.content.Load<Texture2D>("Sprites/Player/run"), 0.1f, true, 12, false);
            jumpanimation = new Animation(level.content.Load<Texture2D>("Sprites/Player/jump"), 0.1f, false, 5, false);
            attack = new Animation(level.content.Load<Texture2D>("Sprites/Player/attack"), 0.1f, false, 6, true);
            dieanimation = new Animation(level.content.Load<Texture2D>("Sprites/Player/die"), 0.1f, false, 3, false);
            sprite.PlayAnimation(idleanimation);
        }
        public void Reset(Vector2 pos)
        {
            position = pos;
            velocity = Vector2.Zero;
            isalive = true;
            sprite.PlayAnimation(idleanimation);
        }
        public void Update(
            GameTime gameTime, KeyboardState keyboardState)
        {
            GetInput(keyboardState);
            ApplyPhysics(gameTime);
            isattacking = sprite.animation.dontinterrupt;
            if (isalive && isonground)
            {
                if (Math.Abs(velocity.X) - 0.02f > 0)
                {
                    sprite.PlayAnimation(runanimation);
                }
                else
                {
                    sprite.PlayAnimation(idleanimation);
                }
            }
            if (wantstoattack && isalive)
            {
                sprite.PlayAnimation(attack);
            }
            wantstoattack = false;
            movement = 0.0f;
            isjumping = false;
        }
        private void GetInput(KeyboardState keyboardState)
        {
            if (Math.Abs(movement) < 0.5f)
            {
                movement = 0.0f;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                dir = FaceDirection.Left;
                movement = -1.0f;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                dir = FaceDirection.Right;
                movement = 1.0f;
            }
            isjumping = keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W);
            wantstoattack = keyboardState.IsKeyDown(Keys.J);
        }
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 previousPosition = position;
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
            velocity.Y = DoJump(velocity.Y, gameTime);
            if (isonground)
            {
                velocity.X *= GroundDragFactor;
            }
            else
            {
                velocity.X *= AirDragFactor;
            }
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);
            position += velocity * elapsed;
            position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));
            HandleCollisions();
            if (position.X == previousPosition.X)
            {
                velocity.X = 0;
            }
            if (position.Y == previousPosition.Y)
            {
                velocity.Y = 0;
            }
            if (inv)
            {
                iframe = iframe + elapsed;
                if (iframe >= maxiframe)
                {
                    inv = false;
                    iframe = 0;
                }
            }
        }
        private float DoJump(float velocityY, GameTime gameTime)
        {
            if (isjumping)
            {
                if ((!wasjumping && isonground) || jumptime > 0.0f)
                {
                    jumptime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpanimation);
                }
                if (0.0f < jumptime && jumptime <= MaxJumpTime)
                {
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumptime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    jumptime = 0.0f;
                }
            }
            else
            {
                jumptime = 0.0f;
            }
            wasjumping = isjumping;
            return velocityY;
        }
        private void HandleCollisions()
        {
            Rectangle bounds = boundingrectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;
            isonground = false;
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    TileCollision collision = level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        Rectangle tileBounds = level.GetBounds(x, y);
                        Vector2 depth = RectangleHelp.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                if (previousbottom <= tileBounds.Top)
                                {
                                    isonground = true;
                                }
                                if (collision == TileCollision.Impassable || isonground)
                                {
                                    position = new Vector2(position.X, position.Y + depth.Y);
                                    bounds = boundingrectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable)
                            {
                                position = new Vector2(position.X + depth.X, position.Y);
                                bounds = boundingrectangle;
                            }
                        }
                    }
                }
            }
            previousbottom = bounds.Bottom;
        }
        public void instkill()
        {
            inv = false;
            iframe = 0;
            hp = 0;
            isalive = false;
        }
        public void Damaged()
        {
            if (!inv)
            {
                hp--;
                if (hp == 0)
                {
                    isalive = false;
                }
                inv = true;
            }
        }
        public void Ondeath()
        {
            sprite.PlayAnimation(dieanimation);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (velocity.X < 0)
            {
                flip = SpriteEffects.FlipHorizontally;
            }
            else if (velocity.X > 0)
            {
                flip = SpriteEffects.None;
            }
            sprite.Draw(gameTime, spriteBatch, position, flip);
        }
    }
}
