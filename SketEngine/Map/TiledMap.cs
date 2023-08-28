using System.Collections.Generic;

namespace Sket.Map
{
	public sealed class TiledMap
	{
		//Main Tiled Variables
		public int Height { get; set; }
		public int Width { get; set; }
		public int TileHeight { get; set; }
		public int TileWidth { get; set; }
		public List<TiledLayer> Layers { get; set; }
		public List<TiledSet> TileSets { get; set; }

		//Other/Extra Tiled Variables
		public int CompressionLevel { get; set; }
		public bool Infinite { get; set; }
		public int NextLayerId { get; set; }
		public int NextObjectId { get; set; }
		public string Orientation { get; set; }
		public string RenderOrder { get; set; }
		public string TiledVersion { get; set; }
		public string Type { get; set; }
		public string Version { get; set; }
	}
}
