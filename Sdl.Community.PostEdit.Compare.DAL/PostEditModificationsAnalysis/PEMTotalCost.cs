using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis
{
	public class PEMTotalCost
	{
		public decimal PostEditExactPrice { get; set; }
		public decimal PostEditP99Price { get; set; }
		public decimal PostEditP94Price { get; set; }
		public decimal PostEditP84Price { get; set; }
		public decimal PostEditP74Price { get; set; }
		public decimal PostEditNewPrice { get; set; }
		public string Currency { get; set; }
	}
}
