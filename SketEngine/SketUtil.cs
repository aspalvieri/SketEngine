using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace Sket
{
	public static class SketUtil
	{
		// 1/10th of a millimeter
		public static readonly float VerySmallAmount = 0.0001f;

		public static void ToggleFullscreen(GraphicsDeviceManager graphics)
		{
			graphics.HardwareModeSwitch = false;
			graphics.ToggleFullScreen();
		}

		public static int Clamp(int value, int min, int max)
		{
			if (min > max) {
				throw new ArgumentOutOfRangeException("The value of 'min' is greater than the value of 'max'.");
			}

			if (value < min) {
				return min;
			}
			else if (value > max) {
				return max;
			}
			return value;
		}

		public static float Clamp(float value, float min, float max)
		{
			if (min > max) {
				throw new ArgumentOutOfRangeException("The value of 'min' is greater than the value of 'max'.");
			}

			if (value < min) {
				return min;
			}
			else if (value > max) {
				return max;
			}
			return value;
		}

		public static void Normalize(ref float x, ref float y)
		{
			float invLen = 1f / MathF.Sqrt((x * x) + (y * y));

			x *= invLen;
			y *= invLen;
		}
	}
}
