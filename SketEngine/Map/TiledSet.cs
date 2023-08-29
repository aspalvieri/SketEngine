using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sket.Map
{
	public sealed class TiledSetProperties
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }
	}

	public sealed class TiledSetTiles
	{
		public int Id { get; set; }
		public List<TiledSetProperties> Properties { get; set; }
	}

	public sealed class TiledSet
	{
		public int FirstGid { get; set; }
		public List<TiledSetTiles> Tiles { get; set; }
		public int TileHeight { get; set; }
		public int TileWidth { get; set; }
		public string Name { get; set; }
	}
}
