using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Sket.Input
{
	public sealed class SketKeyboard
	{
		private static readonly Lazy<SketKeyboard> Lazy = new Lazy<SketKeyboard>(() => new SketKeyboard());

		public static SketKeyboard Instance {
			get { return Lazy.Value; }
		}

		private KeyboardState prevKeyboard;
		private KeyboardState currKeyboard;

		public SketKeyboard()
		{
			prevKeyboard = Keyboard.GetState();
			currKeyboard = prevKeyboard;
		}

		public void Update()
		{
			prevKeyboard = currKeyboard;
			currKeyboard = Keyboard.GetState();
		}

		public bool IsKeyDown(Keys key)
		{
			return currKeyboard.IsKeyDown(key);
		}

		public bool IsKeyClicked(Keys key)
		{
			return currKeyboard.IsKeyDown(key) && !prevKeyboard.IsKeyDown(key);
		}

		public bool IsKeyReleased(Keys key)
		{
			return !currKeyboard.IsKeyDown(key) && prevKeyboard.IsKeyDown(key);
		}
	}
}
