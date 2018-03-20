using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis
{
	public class TERpTotalValues
	{
		public decimal Segments { get; set; }
		public decimal Words { get; set; }
		public decimal RefWords { get; set; }
		public decimal Errors { get; set; }
		public decimal Ins { get; set; }
		public decimal Del { get; set; }
		public decimal Sub { get; set; }
		public decimal Shft { get; set; }
	}
}
