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
		private List<SketTile> shadows;
		private bool isLoaded;

		private int tileLayer;

		private string project; //The name of the Project which will contain the maps

		public SketMap()
		{
			isLoaded = false;
		}

		public SketMap(string project)
		{
			//The name of the Project which will contain the maps
			this.project = project;
		}

		public void LoadMap(string path, Texture2D tilesheet, Texture2D shadowsheet)
		{
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			//  BE SURE TO SET THE map.json TO AN EMBEDDED RESOURCE UNDER IT'S PROPERTIES -> BUILD ACTION
			//	EXAMPLE PATH: "SketGame.Content.maps.map1.json"
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			//Unload the map first to remove any existing references
			Unload();

			//Grab the assembly from the invoking project
			Assembly assembly = Assembly.Load(project);

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
			shadows = new List<SketTile>();
			int tilesheetMaxX = (tilesheet.Width / map.TileWidth);
			int totalTiles = map.Width * map.Height;
			tileLayer = map.Layers.Count - 1;

			//Go through all the layers and build out the maps
			for (int i = 0; i < map.Layers.Count; i++) {
				if (map.Layers[i].Name == "tiles") {
					tileLayer = i;
				}
			}

			int x = 0;
			int y = 0;

			int total = 0;
			for (int v = 0; v < map.Layers[tileLayer].Data.Count; v++) {
				int tile = map.Layers[tileLayer].Data[v];

				tiles.Add(new SketTile(tilesheet, new Vector2(x, y),
					map.TileWidth, map.TileHeight, tile, tilesheetMaxX, map.TileSets));

				x += map.TileWidth;
				if (++total >= map.Width) {
					x = 0;
					y += map.TileHeight;
					total = 0;
				}
			}

			for (int i = 0; i < totalTiles; i++) {
				shadows.Add(new SketTile());
			}

			//Calculate shadows for the tiles
			for (y = 1; y < map.Height - 1; y++) {
				int yIndex = y * map.Width;
				int ym1 = yIndex - (1 * map.Width); //y minus 1
				int yp1 = yIndex + (1 * map.Width); //y plus 1
				for (x = 1; x < map.Width - 1; x++) {
					if (tiles[x + yIndex].CastShadow == true) {
						//No tile above the tile, and no tile to the left of it
						if (tiles[x + ym1].TakeShadow == true && tiles[(x - 1) + yIndex].CastShadow == false) {
							shadows[x + ym1] = new SketTile(shadowsheet, new Vector2(tiles[x + ym1].Position.X, tiles[x + ym1].Position.Y),
								map.TileWidth, map.TileHeight, new Rectangle(32, 0, map.TileWidth, map.TileHeight));
						}
						//No tile above, and yes tile to the left
						if (tiles[x + ym1].TakeShadow == true && tiles[(x - 1) + yIndex].CastShadow == true) {
							shadows[x + ym1] = new SketTile(shadowsheet, new Vector2(tiles[x + ym1].Position.X, tiles[x + ym1].Position.Y),
								map.TileWidth, map.TileHeight, new Rectangle(0, 0, map.TileWidth, map.TileHeight));
						}
						//No tile above, no tile to right
						if (tiles[x + ym1].TakeShadow == true && tiles[(x + 1) + yIndex].TakeShadow == true && tiles[(x + 1) + ym1].TakeShadow == true) {
							shadows[(x + 1) + ym1] = new SketTile(shadowsheet, new Vector2(tiles[(x + 1) + ym1].Position.X, tiles[(x + 1) + ym1].Position.Y),
								map.TileWidth, map.TileHeight, new Rectangle(64, 0, map.TileWidth, map.TileHeight));
						}
						//No tile the right of the tile, and no tile below it
						if (tiles[(x + 1) + yIndex].TakeShadow == true && tiles[x + yp1].CastShadow == false) {
							shadows[(x + 1) + yIndex] = new SketTile(shadowsheet, new Vector2(tiles[(x + 1) + yIndex].Position.X, tiles[(x + 1) + yIndex].Position.Y),
								map.TileWidth, map.TileHeight, new Rectangle(32, 32, map.TileWidth, map.TileHeight));
						}
						//To the right of the tile, and yes tile below it
						if (tiles[(x + 1) + yIndex].TakeShadow == true && tiles[x + yp1].CastShadow == true) {
							shadows[(x + 1) + yIndex] = new SketTile(shadowsheet, new Vector2(tiles[(x + 1) + yIndex].Position.X, tiles[(x + 1) + yIndex].Position.Y),
								map.TileWidth, map.TileHeight, new Rectangle(0, 32, map.TileWidth, map.TileHeight));
						}
						//No tile above, Tile top-left
						if (tiles[x + ym1].TakeShadow == true && tiles[(x - 1) + ym1].CastShadow == true) {
							shadows[x + ym1] = new SketTile(shadowsheet, new Vector2(tiles[x + ym1].Position.X, tiles[x + ym1].Position.Y),
								map.TileWidth, map.TileHeight, new Rectangle(64, 32, map.TileWidth, map.TileHeight));
						}
					}
				}
			}

			isLoaded = true;
		}

		public void Unload()
		{
			tiles?.Clear();
			tiles = null;
			shadows?.Clear();
			shadows = null;
			map = null;
			tileLayer = -1;
			isLoaded = false;
		}

		public void DrawTiles(SpriteRenderer spriteBatch, Camera camera)
		{
			if (!isLoaded)
				throw new Exception("Map hasn't been loaded yet.");

			camera.GetExtents(out int left, out int right, out int top, out int bottom);
			
			left = SketUtil.Clamp(left / map.TileWidth, 0, map.Width - 1);
			right = SketUtil.Clamp(right / map.TileWidth, 0, map.Width - 1);
			top = SketUtil.Clamp(top / map.TileHeight, 0, map.Height - 1);
			bottom = SketUtil.Clamp(bottom / map.TileHeight, 0, map.Height - 1);

			//left += 1;
			//right -= 1;
			//top += 1;
			//bottom -= 1;

			//Draw tiles
			for (int y = top; y <= bottom; y++) {
				int yIndex = y * map.Width;
				for (int x = left; x <= right; x++) {
					tiles[x + yIndex].Draw(spriteBatch, camera);
				}
			}

			//Draw shadows
			for (int y = top; y <= bottom; y++) {
				int yIndex = y * map.Width;
				for (int x = left; x <= right; x++) {
					shadows[x + yIndex].Draw(spriteBatch, camera);
				}
			}
		}
	}
}
