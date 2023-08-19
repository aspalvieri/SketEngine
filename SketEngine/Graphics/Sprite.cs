using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sket.Graphics;
using System;

namespace Sket
{
    public class SpriteManager
    {
        public Vector2 position = Vector2.Zero;
        public Color color = Color.White;
        public Vector2 origin;
        public float rotation = 0f;
        public float scale = 1f;
        public float alpha = 1f;
        public SpriteEffects spriteEffect;

		protected int width = 0;
		protected int height = 0;
        protected Vector2 center;

		protected Texture2D texture;
        protected Rectangle[] rectangles;
        protected int frameIndex = 0;

        public SpriteManager(Texture2D texture, int frames)
        {
            this.texture = texture;
            int textureWidth = texture.Width / frames;
            rectangles = new Rectangle[frames];

            for (int i = 0; i < frames; i++)
                rectangles[i] = new Rectangle(i * textureWidth, 0, textureWidth, texture.Height);

            center = new Vector2(width / 2.0f, height / 2.0f);
        }

        public void Draw(SpriteRenderer spriteBatch, Camera camera = null)
        {
            if (camera is null) {
				spriteBatch.Draw(texture, rectangles[frameIndex],
					new Rectangle((int)position.X, (int)position.Y, width, height), rotation, origin, color * alpha);
			}
            else {
                spriteBatch.Draw(texture, rectangles[frameIndex],
                    new Rectangle((int)(position.X - camera.Position.X), (int)(position.Y - camera.Position.Y), width, height), rotation, origin, color * alpha);
            }
		}
    }

    public class Sprite : SpriteManager
    {
        public bool isLooping;

        private float timeElapsed;
        private float timeToUpdate; //default, you may have to change it

        public int FramesPerSecond 
        { 
            set { timeToUpdate = (1f / value); } 
        }

        public Sprite(Texture2D texture, int frames, int fps, bool isLooping = true) : base(texture, frames) 
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

        public void SetFrame(int frame)
        {
            frameIndex = frame;
        }

		public Sprite SetSize(int width, int height)
		{
			this.width = width;
			this.height = height;
			return this;
		}
	}
}