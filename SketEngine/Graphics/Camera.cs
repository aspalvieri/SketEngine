using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sket.Graphics
{
	public sealed class Camera
	{
		public readonly static float MinZ = 1f;
		public readonly static float MaxZ = 2048f;

		public readonly static float MinZoom = 2f / 3f;
		public readonly static float MaxZoom = 2f;
		public readonly static float ZoomAmount = 1.0f / 3.0f;
		
		private Vector2 position;
		private float baseZ;
		private float z;

		private float aspectRatio;
		private float fieldOfView;

		private Matrix view;
		private Matrix proj;

		private float zoom;

		public Vector2 Position {
			get { return position; }
		}

		public float Z {
			get { return z; }
		}

		public Matrix View {
			get { return view; }
		}

		public Matrix Projection {
			get { return proj; }
		}

		public Camera(Screen screen)
		{
			if (screen is null)
				throw new ArgumentNullException("screen");

			aspectRatio = (float)screen.Width / screen.Height;
			fieldOfView = MathHelper.PiOver2;

			position = new Vector2(screen.Width / 2, screen.Height / 2);
			baseZ = GetZFromHeight(screen.Height);
			z = baseZ;

			UpdateMatrices();

			zoom = 1;
		}

		public void UpdateMatrices()
		{
			//view = Matrix.CreateLookAt(new Vector3(position.X, position.Y, -z), new Vector3(position.X, position.Y, 0), Vector3.Down);
			view = Matrix.CreateLookAt(new Vector3(0, 0, -z), Vector3.Zero, Vector3.Down);
			proj = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, MinZ, MaxZ);
		}

		public float GetZFromHeight(float height)
		{
			return (0.5f * height) / MathF.Tan(0.5f * fieldOfView);
		}

		public float GetHeightFromZ()
		{
			return z * MathF.Tan(0.5f * fieldOfView) * 2f;
		}

		public void MoveZ(float amount)
		{
			z += amount;
			z = SketUtil.Clamp(z, MinZ, MaxZ);
		}

		public void ResetZ()
		{
			z = baseZ;
		}

		public void Move(Vector2 amount)
		{
			position += amount;
		}

		public void MoveTo(Vector2 position)
		{
			this.position = position;
		}

		public void IncZoom()
		{
			zoom += ZoomAmount;
			zoom = SketUtil.Clamp(zoom, MinZoom, MaxZoom);
			z = MathF.Round(baseZ / zoom);
		}

		public void DecZoom()
		{
			zoom -= ZoomAmount;
			zoom = SketUtil.Clamp(zoom, MinZoom, MaxZoom);
			z = MathF.Round(baseZ / zoom);
		}

		public void SetZoom(float amount)
		{
			zoom = amount;
			zoom = SketUtil.Clamp(zoom, MinZoom, MaxZoom);
			z = MathF.Round(baseZ / zoom);
		}

		public void GetExtents(out float width, out float height)
		{
			height = GetHeightFromZ();
			width = height * aspectRatio;
		}

		public void GetExtents(out float left, out float right, out float top, out float bottom)
		{
			GetExtents(out float width, out float height);

			left = position.X - (width * 0.5f);
			right = left + width;
			top = position.Y - (height * 0.5f);
			bottom = top + height;
		}

		public void GetExtents(out Vector2 min, out Vector2 max)
		{
			GetExtents(out float left, out float right, out float top, out float bottom);

			min = new Vector2(left, top);
			max = new Vector2(right, bottom);
		}
	}
}
