using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis
{
	public class PEMResults
	{
		public decimal SegmentsNo { get; set; }
		public decimal WordsNo { get; set; }
		public decimal CharactersNo { get; set; }
		public decimal Percent { get; set; }
		public decimal Total { get; set; }
		public decimal Tags { get; set; }
	}
}
