using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sket.Graphics;
using System;

namespace Sket.Input
{
	public sealed class SketMouse
	{
		private static readonly Lazy<SketMouse> Lazy = new Lazy<SketMouse>(() => new SketMouse());

		public static SketMouse Instance {
			get { return Lazy.Value; }
		}

		private MouseState prevMouse;
		private MouseState currMouse;

		public Point WindowPosition {
			get { return currMouse.Position; }
		}

		public SketMouse()
		{
			prevMouse = Mouse.GetState();
			currMouse = prevMouse;
		}

		public void Update()
		{
			prevMouse = currMouse;
			currMouse = Mouse.GetState();
		}

		public Vector2 GetScreenPosition(Screen screen)
		{
			Rectangle screenDestinationRectangle = screen.CalculateDestinationRectangle();

			Point windowPosition = WindowPosition;

			float sx = windowPosition.X - screenDestinationRectangle.X;
			float sy = windowPosition.Y - screenDestinationRectangle.Y;

			sx /= (float)screenDestinationRectangle.Width;
			sy /= (float)screenDestinationRectangle.Height;

			sx *= screen.Width;
			sy *= screen.Height;

			return new Vector2(sx, sy);
		}

		public bool IsLeftButtonDown()
		{
			return currMouse.LeftButton == ButtonState.Pressed;
		}

		public bool IsRightButtonDown()
		{
			return currMouse.RightButton == ButtonState.Pressed;
		}

		public bool IsMiddleButtonDown()
		{
			return currMouse.MiddleButton == ButtonState.Pressed;
		}

		public bool IsLeftButtonClicked()
		{
			return currMouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
		}

		public bool IsRightButtonClicked()
		{
			return currMouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released;
		}

		public bool IsMiddleButtonClicked()
		{
			return currMouse.MiddleButton == ButtonState.Pressed && prevMouse.MiddleButton == ButtonState.Released;
		}

		public bool IsLeftButtonReleased()
		{
			return currMouse.LeftButton == ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed;
		}

		public bool IsRightButtonReleased()
		{
			return currMouse.RightButton == ButtonState.Released && prevMouse.RightButton == ButtonState.Pressed;
		}

		public bool IsMiddleButtonReleased()
		{
			return currMouse.MiddleButton == ButtonState.Released && prevMouse.MiddleButton == ButtonState.Pressed;
		}
	}
}
