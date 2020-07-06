using System;
using System.Xml.Serialization;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WordCount : ICloneable
	{	
		[XmlAttribute]
		public string Category { get; set; }

		[XmlAttribute]
		public int Segments { get; set; }

		[XmlAttribute]
		public int Words { get; set; }

		[XmlAttribute]
		public int Characters { get; set; }

		[XmlAttribute]
		public int Placeables { get; set; }

		[XmlAttribute]
		public int Tags { get; set; }		

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
