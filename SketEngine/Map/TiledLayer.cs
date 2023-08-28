using System.Collections.Generic;

namespace Sket.Map
{
	public sealed class TiledLayer
	{
		//Main Tiled Variables
		public int Height { get; set; }
		public int Width { get; set; }
		public List<int> Data { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }

		//Other/Extra Tiled Variables
		public int Opacity { get; set; }
		public string Type { get; set; }
		public bool Visible { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
	}
}
