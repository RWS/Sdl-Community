using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis
{
	public class PEMModel
	{
		public PEMResults Hundred { get; set; }
		public PEMResults Fuzzy99 { get; set; }
		public PEMResults Fuzzy94 { get; set; }
		public PEMResults Fuzzy84 { get; set; }
		public PEMResults Fuzzy74 { get; set; }
		public PEMResults New { get; set; }
		public PEMResults Total { get; set; }

	}

}
