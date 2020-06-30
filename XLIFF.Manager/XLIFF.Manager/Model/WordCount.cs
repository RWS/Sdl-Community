using System;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WordCount : ICloneable
	{
		public string Category { get; set; }

		public int Segments { get; set; }
		
		public int Words { get; set; }
		
		public int Characters { get; set; }
		
		public int Placeables { get; set; }
		
		public int Tags { get; set; }

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
