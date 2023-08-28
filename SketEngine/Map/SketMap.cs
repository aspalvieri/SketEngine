using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Sket.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sket.Map
{
	public sealed class SketMap
	{
		private TiledMap map;
		private List<SketTile> tiles;
		private Texture2D tilesheet;
		private bool isLoaded;

		private int tilesheetMaxX;

		public SketMap()
		{
			isLoaded = false;
		}

		public void LoadMap(string path, Texture2D tilesheet)
		{
			this.tilesheet = tilesheet;

			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//  BE SURE TO SET THE map.json TO AN EMBEDDED RESOURCE UNDER IT'S PROPERTIES -> BUILD ACTION
			//	EXAMPLE PATH: "SketGame.Content.maps.map1.json"
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			Assembly assembly = Assembly.GetCallingAssembly();

			// Open a stream to the embedded resource
			Stream stream = assembly.GetManifestResourceStream(path);
			if (stream is null)
				throw new ArgumentNullException("stream");

			// Read the contents of the resource
			StreamReader reader = new StreamReader(stream);
			string resourceContent = reader.ReadToEnd();
			map = JsonConvert.DeserializeObject<TiledMap>(resourceContent);

			//Declare tiles array and max index for tilesheet
			tiles = new List<SketTile>();
			tilesheetMaxX = (tilesheet.Width / map.TileWidth);

			//Go through all the layers and build out the maps
			for (int i = 0; i < map.Layers.Count; i++) {
				int x = 0;
				int y = 0;
				int total = 0;
				for (int v = 0; v < map.Layers[i].Data.Count; v++) {
					int tile = map.Layers[i].Data[v] - 1;
					int tileSpacer = tile / tilesheetMaxX;
					int tileX = (tile - (tileSpacer * tilesheetMaxX)) * map.TileWidth;
					int tileY = tileSpacer * map.TileHeight;

					tiles.Add(new SketTile(tilesheet, new Vector2(x, y),
						new Rectangle(tileX, tileY, map.TileWidth, map.TileHeight),
						map.TileWidth, map.TileHeight));

					x += map.TileWidth;
					if (++total >= map.Width) {
						x = 0;
						y += map.TileHeight;
						total = 0;
					}
				}
			}

			isLoaded = true;
		}

		public void DrawTiles(SpriteRenderer spriteBatch, Camera camera)
		{
			if (!isLoaded)
				throw new Exception("Map hasn't been loaded yet.");

			camera.GetExtents(out float left, out float right, out float top, out float bottom);
			
			left = SketUtil.Clamp(left / map.TileWidth, 0, map.Width - 1);
			right = SketUtil.Clamp(right / map.TileWidth, 0, map.Width - 1);
			top = SketUtil.Clamp(top / map.TileHeight, 0, map.Height - 1);
			bottom = SketUtil.Clamp(bottom / map.TileHeight, 0, map.Height - 1);

			//left += 1;
			//right -= 1;
			//top += 1;
			//bottom -= 1;

			for (int i = 0; i < map.Layers.Count; i++) {
				for (int y = (int)top; y <= (int)bottom; y++) {
					int yIndex = y * map.Width;
					for (int x = (int)left; x <= (int)right; x++) {
						tiles[x + yIndex].Draw(spriteBatch, camera);
					}
				}
			}
		}
	}
}
