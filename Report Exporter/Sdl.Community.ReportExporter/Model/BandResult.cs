using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ReportExporter.Model
{
	public class BandResult
	{
		public string BandName { get; }
		public int Segments { get; set; }
		public int Words { get; set; }
		public int Characters { get; set; }
		public int Placeables { get; set; }
		public int Tags { get; set; }
		public int Min { get; set; }
		public int Max { get; set; }

		public BandResult(string bandName)
		{
			BandName = bandName;
		}
	}
}
