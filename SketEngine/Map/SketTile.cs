using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sket.Graphics;
using System;
using System.Collections.Generic;

namespace Sket.Map
{
	public sealed class SketTile
	{
		private Vector2 position;
		private Rectangle clip;
		private Texture2D tilesheet;
		private int width;
		private int height;
		private int id;
		private List<TiledSetProperties> properties;
		private bool collision;
		private bool castShadow;
		private bool takeShadow;
		private bool takeSunlight;

		private bool enabled;

		public Vector2 Position {
			get { return position; }
		}
		public bool Collision { 
			get { return collision; }
		}
		public bool CastShadow { 
			get { return castShadow; }
		}
		public bool TakeShadow { 
			get { return takeShadow; }
		}
		public bool TakeSunlight {
			get { return takeSunlight; }
		}

		public SketTile()
		{
			enabled = false;
		}

		public SketTile(Texture2D tilesheet, Vector2 position, int width, int height, int id, int tilesheetMaxX, List<TiledSet> tileset)
		{
			this.tilesheet = tilesheet;
			this.position = position;
			this.width = width;
			this.height = height;
			this.id = id;

			//Load all of the custom properties for the tile
			FindProperties(tileset);

			int tileSpacer = this.id / tilesheetMaxX;
			int tileX = (this.id - (tileSpacer * tilesheetMaxX)) * width;
			int tileY = tileSpacer * height;
			clip = new Rectangle(tileX, tileY, width, height);

			string collisionStr = FindProperty("collision");
			collision = bool.Parse(collisionStr);

			string castShadowStr = FindProperty("castShadow");
			castShadow = bool.Parse(castShadowStr);

			string takeShadowStr = FindProperty("takeShadow");
			takeShadow = bool.Parse(takeShadowStr);

			string takeSunlightStr = FindProperty("takeSunlight");
			takeSunlight = bool.Parse(takeSunlightStr);

			enabled = true;
		}

		public SketTile(Texture2D tilesheet, Vector2 position, int width, int height, Rectangle clip)
		{
			this.tilesheet = tilesheet;
			this.position = position;
			this.width = width;
			this.height = height;
			this.clip = clip;

			collision = false;
			castShadow = false;
			takeShadow = false;
			takeSunlight = true;

			enabled = true;
		}

		private void FindProperties(List<TiledSet> tileset)
		{
			//Tiles are required at least 1 custom property since it's how it matches the proper id to subtract
			foreach (TiledSet tset in tileset) {
				foreach (TiledSetTiles tsetTiles in tset.Tiles) {
					if (tsetTiles.Id == id - tset.FirstGid) {
						id -= tset.FirstGid;
						properties = tsetTiles.Properties;
						return;
					}
				}
			}
			if (properties is null)
				throw new ArgumentNullException("properties");
		}

		private string FindProperty(string name)
		{
			foreach (TiledSetProperties prop in properties) {
				if (prop.Name == name) {
					return prop.Value;
				}
			}
			throw new Exception("Property '" + name + "' not found.");
		}

		public void Draw(SpriteRenderer spriteBatch, Camera camera)
		{
			if (enabled == true) {
				float alpha = 1f;
				if (takeSunlight == false) {
					alpha = 0.3f;
				}
				Rectangle destination = new Rectangle((int)(position.X - camera.Position.X), (int)(position.Y - camera.Position.Y), width, height);
				spriteBatch.Draw(tilesheet, clip, destination, Color.White * alpha);
			}
		}
	}
}
