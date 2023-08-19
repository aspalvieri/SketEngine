﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Sket.Graphics
{
	public sealed class Screen : IDisposable
	{
		private readonly static int MinDimension = 64;
		private readonly static int MaxDimension = 4096;

		private bool isDisposed;
		private Game game;
		private RenderTarget2D target;
		private bool isSet;

		public int Width {
			get { return target.Width; }
		}

		public int Height {
			get { return target.Height; }
		}

		public Screen(Game game, int width, int height)
		{
			width = SketUtil.Clamp(width, MinDimension, MaxDimension);
			height = SketUtil.Clamp(height, MinDimension, MaxDimension);

			if (game is null)
				throw new ArgumentNullException("game");

			this.game = game;
			target = new RenderTarget2D(this.game.GraphicsDevice, width, height);
			isSet = false;
		}

		public void Dispose()
		{
			if (isDisposed) 
				return;

			target?.Dispose();
			isDisposed = true;
		}

		public void Set()
		{
			if (isSet)
				throw new Exception("Render target is already set.");

			game.GraphicsDevice.SetRenderTarget(target);
			isSet = true;
		}

		public void UnSet()
		{
			if (!isSet)
				throw new Exception("Render target is not set.");

			game.GraphicsDevice.SetRenderTarget(null);
			isSet = false;
		}

		public void Present(Sprites sprites, bool textureFiltering = true)
		{
			if (sprites is null)
				throw new ArgumentNullException("sprites");

#if DEBUG
			game.GraphicsDevice.Clear(Color.HotPink);
#else
			game.GraphicsDevice.Clear(Color.Black);
#endif

			Rectangle destination = CalculateDestinationRectangle();

			sprites.Begin(null, textureFiltering);
			sprites.Draw(target, null, destination, Color.White);
			sprites.End();
		}

		internal Rectangle CalculateDestinationRectangle()
		{
			Rectangle backbufferBounds = game.GraphicsDevice.PresentationParameters.Bounds;
			float backbufferAspectRatio = (float)backbufferBounds.Width / backbufferBounds.Height;
			float screenAspectRatio = (float)Width / Height;

			float rx = 0f;
			float ry = 0f;
			float rw = backbufferBounds.Width;
			float rh = backbufferBounds.Height;

			if (backbufferAspectRatio > screenAspectRatio) {
				rw = rh * screenAspectRatio;
				rx = ((float)backbufferBounds.Width - rw) / 2f;
			}
			else if (backbufferAspectRatio < screenAspectRatio) {
				rh = rw / screenAspectRatio;
				ry = ((float)backbufferBounds.Height - rh) / 2f;
			}

			Rectangle result = new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
			return result;
		}
	}
}
