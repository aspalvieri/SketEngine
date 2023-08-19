using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sket.Graphics;

namespace Sket
{
    public class SpriteManager
    {
        public Vector2 position = Vector2.Zero;
        public Color color = Color.White;
        public Vector2 origin;
        public float rotation = 0f;
        public float scale = 1f;
        public SpriteEffects spriteEffect;

        protected Texture2D texture;
        protected Rectangle[] rectangles;
        protected int frameIndex = 0;

        public SpriteManager(Texture2D texture, int frames)
        {
            this.texture = texture;
            int width = texture.Width / frames;
            rectangles = new Rectangle[frames];

            for (int i = 0; i < frames; i++)
                rectangles[i] = new Rectangle(i * width, 0, width, texture.Height);
        }

        public void Draw(Sprites spriteBatch, Camera camera = null)
        {
            if (camera is null) {
                spriteBatch.Draw(texture, position, rectangles[frameIndex], rotation, origin, scale, color);
            }
            else {
                spriteBatch.Draw(texture, new Vector2(position.X - camera.Position.X, position.Y - camera.Position.Y), rectangles[frameIndex], rotation, origin, scale, color);
            }
		}
    }

    public class SpriteAnimation : SpriteManager
    {
        public bool isLooping;

        private float timeElapsed;
        private float timeToUpdate; //default, you may have to change it

        public int FramesPerSecond 
        { 
            set { timeToUpdate = (1f / value); } 
        }

        public SpriteAnimation(Texture2D texture, int frames, int fps, bool isLooping = true) : base(texture, frames) 
        {
            FramesPerSecond = fps;
            this.isLooping = isLooping;
        }

        public void Update(GameTime gameTime)
        {
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;

                if (frameIndex < rectangles.Length - 1)
                    frameIndex++;
                else if (isLooping)
                    frameIndex = 0;
            }
        }

        public void setFrame(int frame)
        {
            frameIndex = frame;
        }
    }
}