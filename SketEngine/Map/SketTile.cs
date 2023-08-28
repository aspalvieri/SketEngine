using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sket.Graphics;
using System;

namespace Sket.Map
{
	public sealed class SketTile
	{
		public Vector2 position;
		private Rectangle clip;
		private Texture2D tilesheet;
		private int width;
		private int height;

		public SketTile(Texture2D tilesheet, Vector2 position, Rectangle clip, int width, int height)
		{
			this.tilesheet = tilesheet;
			this.position = position;
			this.clip = clip;
			this.width = width;
			this.height = height;
		}

		public void Draw(SpriteRenderer spriteBatch, Camera camera)
		{
			Rectangle destination = new Rectangle((int)(position.X - camera.Position.X), (int)(position.Y - camera.Position.Y), width, height);
			spriteBatch.Draw(tilesheet, clip, destination, Color.White);
		}
	}
}
