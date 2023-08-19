using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Sket.Graphics
{
	public sealed class ShapeRenderer : IDisposable
	{
		public static readonly float MinLineThickness = 1f;
		public static readonly float MaxLineThickness = 10f;

		private Game game;
		private BasicEffect effect;
		private VertexPositionColor[] vertices;
		private int[] indices;

		private int shapeCount;
		private int vertexCount;
		private int indexCount;

		private bool isStarted;
		private bool isDisposed;

		public ShapeRenderer(Game game)
		{
			if (game is null)
				throw new ArgumentNullException("game");

			this.game = game;

			effect = new BasicEffect(game.GraphicsDevice);
			effect.TextureEnabled = false;
			effect.FogEnabled = false;
			effect.LightingEnabled = false;
			effect.VertexColorEnabled = true;
			effect.World = Matrix.Identity;
			effect.View = Matrix.Identity;
			effect.Projection = Matrix.Identity;

			const int MaxVertexCount = 1024;
			const int MaxIndexCount = MaxVertexCount * 3;

			vertices = new VertexPositionColor[MaxVertexCount];
			indices = new int[MaxIndexCount];

			shapeCount = 0;
			vertexCount = 0;
			indexCount = 0;

			isStarted = false;
			isDisposed = false;
		}

		public void Dispose()
		{
			if (isDisposed) {
				return;
			}

			effect?.Dispose();
			isDisposed = true;
		}

		public void Begin(Camera camera)
		{
			if (isStarted) {
				throw new Exception("Batching is already started.");
			}

			if (camera is null) {
				Viewport viewport = game.GraphicsDevice.Viewport;
				effect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0f, 1f);
				effect.View = Matrix.Identity;
			}
			else {
				camera.UpdateMatrices();
				effect.View = camera.View;
				effect.Projection = camera.Projection;
			}

			isStarted = true;
		}

		public void End()
		{
			Flush();
			isStarted = false;
		}

		public void Flush()
		{
			if (shapeCount == 0) {
				return;
			}

			EnsureStarted();

			foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
				pass.Apply();
				game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
					PrimitiveType.TriangleList,
					vertices,
					0,
					vertexCount,
					indices,
					0,
					indexCount / 3
				);
			}

			shapeCount = 0;
			vertexCount = 0;
			indexCount = 0;
		}

		private void EnsureStarted()
		{
			if (!isStarted) {
				throw new Exception("Batching was never started.");
			}
		}

		private void EnsureSpace(int shapeVertexCount, int shapeIndexCount)
		{
			if (shapeVertexCount > vertices.Length) {
				throw new Exception("Maximum shape vertex count is: " + vertices.Length);
			}
			if (shapeIndexCount > indices.Length) {
				throw new Exception("Maximum shape index count is: " + indices.Length);
			}

			if ((vertexCount + shapeVertexCount > vertices.Length) || (indexCount + shapeIndexCount > indices.Length)) {
				Flush();
			}
		}

		public void DrawRectangle(float x, float y, float width, float height, float thickness, Color color)
		{
			float left = x;
			float right = x + width;
			float top = y;
			float bottom = y + height;

			DrawLine(left, top, right, top, thickness, color);
			DrawLine(right, top, right, bottom, thickness, color);
			DrawLine(right, bottom, left, bottom, thickness, color);
			DrawLine(left, bottom, left, top, thickness, color);
		}

		public void DrawRectangleFill(float x, float y, float width, float height, Color color)
		{
			EnsureStarted();

			const int shapeVertexCount = 4;
			const int shapeIndexCount = 6;

			EnsureSpace(shapeVertexCount, shapeIndexCount);

			float left = x;
			float right = x + width;
			float top = y;
			float bottom = y + height;

			//Vector2 a = new Vector2(left, top);
			//Vector2 b = new Vector2(right, top);
			//Vector2 c = new Vector2(right, bottom);
			//Vector2 d = new Vector2(left, bottom);

			indices[indexCount++] = 0 + vertexCount;
			indices[indexCount++] = 1 + vertexCount;
			indices[indexCount++] = 2 + vertexCount;
			indices[indexCount++] = 0 + vertexCount;
			indices[indexCount++] = 2 + vertexCount;
			indices[indexCount++] = 3 + vertexCount;

			//vertices[vertexCount++] = new VertexPositionColor(new Vector3(a, 0f), color);
			//vertices[vertexCount++] = new VertexPositionColor(new Vector3(b, 0f), color);
			//vertices[vertexCount++] = new VertexPositionColor(new Vector3(c, 0f), color);
			//vertices[vertexCount++] = new VertexPositionColor(new Vector3(d, 0f), color);

			vertices[vertexCount++] = new VertexPositionColor(new Vector3(left, top, 0f), color);
			vertices[vertexCount++] = new VertexPositionColor(new Vector3(right, top, 0f), color);
			vertices[vertexCount++] = new VertexPositionColor(new Vector3(right, bottom, 0f), color);
			vertices[vertexCount++] = new VertexPositionColor(new Vector3(left, bottom, 0f), color);

			shapeCount++;
		}

		public void DrawLine(Vector2 a, Vector2 b, float thickness, Color color)
		{
			DrawLine(a.X, a.Y, b.X, b.Y, thickness, color);
		}

		public void DrawLine(float ax, float ay, float bx, float by, float thickness, Color color)
		{
			EnsureStarted();

			const int shapeVertexCount = 4;
			const int shapeIndexCount = 6;

			EnsureSpace(shapeVertexCount, shapeIndexCount);

			thickness = SketUtil.Clamp(thickness, MinLineThickness, MaxLineThickness);

			float halfThickness = thickness / 2;

			float e1x = bx - ax;
			float e1y = by - ay;

			SketUtil.Normalize(ref e1x, ref e1y);

			e1x *= halfThickness;
			e1y *= halfThickness;

			float e2x = -e1x;
			float e2y = -e1y;

			float n1x = -e1y;
			float n1y = e1x;
			
			float n2x = -n1x;
			float n2y = -n1y;

			float q1x = ax + n1x + e2x;
			float q1y = ay + n1y + e2y;

			float q2x = bx + n1x + e1x;
			float q2y = by + n1y + e1y;

			float q3x = bx + n2x + e1x;
			float q3y = by + n2y + e1y;

			float q4x = ax + n2x + e2x;
			float q4y = ay + n2y + e2y;

			indices[indexCount++] = 0 + vertexCount;
			indices[indexCount++] = 1 + vertexCount;
			indices[indexCount++] = 2 + vertexCount;
			indices[indexCount++] = 0 + vertexCount;
			indices[indexCount++] = 2 + vertexCount;
			indices[indexCount++] = 3 + vertexCount;

			vertices[vertexCount++] = new VertexPositionColor(new Vector3(q1x, q1y, 0f), color);
			vertices[vertexCount++] = new VertexPositionColor(new Vector3(q2x, q2y, 0f), color);
			vertices[vertexCount++] = new VertexPositionColor(new Vector3(q3x, q3y, 0f), color);
			vertices[vertexCount++] = new VertexPositionColor(new Vector3(q4x, q4y, 0f), color);

			shapeCount++;
		}

		public void DrawCircle(float x, float y, float radius, int points, float thickness, Color color)
		{
			const int minPoints = 8;
			const int maxPoints = 256;

			points = SketUtil.Clamp(points, minPoints, maxPoints);

			float rotation = MathHelper.TwoPi / (float)points;

			float sin = MathF.Sin(rotation);
			float cos = MathF.Cos(rotation);

			float ax = radius;
			float ay = 0f;

			float bx = 0f;
			float by = 0f;

			for (int i = 0; i < points; i++) {
				bx = (cos * ax) - (sin * ay);
				by = (sin * ax) + (cos * ay);

				DrawLine(ax + x, ay + y, bx + x, by + y, thickness, color);

				ax = bx;
				ay = by;
			}
		}

		public void DrawCircleFill(float x, float y, float radius, int points, Color color)
		{
			EnsureStarted();

			const int minPoints = 3;
			const int maxPoints = 256;

			int shapeVertexCount = SketUtil.Clamp(points, minPoints, maxPoints);
			int shapeTriangleCount = shapeVertexCount - 2;
			int shapeIndexCount = shapeTriangleCount * 3;

			EnsureSpace(shapeVertexCount, shapeIndexCount);

			int index = 1;

			for (int i = 0; i < shapeTriangleCount; i++) {
				indices[indexCount++] = 0 + vertexCount;
				indices[indexCount++] = index + vertexCount;
				indices[indexCount++] = index + 1 + vertexCount;

				index++;
			}

			float rotation = MathHelper.TwoPi / (float)points;

			float sin = MathF.Sin(rotation);
			float cos = MathF.Cos(rotation);

			float ax = radius;
			float ay = 0f;

			for (int i = 0; i < shapeVertexCount; i++) {
				float x1 = ax;
				float y1 = ay;

				vertices[vertexCount++] = new VertexPositionColor(new Vector3(x1 + x, y1 + y, 0f), color);

				ax = (cos * x1) - (sin * y1);
				ay = (sin * x1) + (cos * y1);
			}

			shapeCount++;
		}

		public void DrawPolygon(Vector2[] vertices, Matrix transform, float thickness, Color color)
		{
			for (int i = 0; i < vertices.Length; i++) {
				Vector2 a = vertices[i];
				Vector2 b = vertices[(i + 1) % vertices.Length];

				a = Vector2.Transform(a, transform);
				b = Vector2.Transform(b, transform);

				DrawLine(a, b, thickness, color);
			}
		}

		public void DrawPolygonFill(Vector2[] vertices, int[] triangleIndices, Matrix transform, Color color)
		{
#if DEBUG
			if (vertices is null)
				throw new ArgumentNullException("vertices");

			if (triangleIndices is null)
				throw new ArgumentNullException("indices");

			if (vertices.Length < 3)
				throw new ArgumentOutOfRangeException("vertices");

			if (indices.Length < 3)
				throw new ArgumentOutOfRangeException("indices");
#endif
			EnsureStarted();
			EnsureSpace(vertices.Length, triangleIndices.Length);

			for (int i = 0; i < triangleIndices.Length; i++) {
				indices[indexCount++] = triangleIndices[i] + vertexCount;
			}

			for (int i = 0; i < vertices.Length; i++) {
				Vector2 vertex = vertices[i];
				vertex = Vector2.Transform(vertex, transform);

				this.vertices[vertexCount++] = new VertexPositionColor(new Vector3(vertex.X, vertex.Y, 0f), color);
			}

			shapeCount++;
		}
	}
}
