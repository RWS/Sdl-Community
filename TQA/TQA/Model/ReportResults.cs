using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TQA.Model
{
	public class ReportResults
	{
		public List<Entry> Entries { get; set; }
		public List<string> EvaluationComments { get; set; }
	}
}
