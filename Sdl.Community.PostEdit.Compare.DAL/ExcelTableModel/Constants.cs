using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel
{
	public static class Constants
	{
		#region Post edit modicifations analysis table
		public static string ExactMatch { get { return "100%"; } }
		public static string Segments { get { return "Segments"; } }
		public static string Words { get { return "Words"; } }
		public static string Characters { get { return "Characters"; } }
		public static string Percent { get { return "Percent"; } }
		public static string Total { get { return "Total"; } }
		public static string Fuzzy99 { get { return "95% - 99%"; } }
		public static string Fuzzy94 { get { return "85% - 94%"; } }
		public static string Fuzzy84 { get { return "75% - 84%"; } }
		public static string Fuzzy74 { get { return "50% - 74%"; } }
		public static string New { get { return "New"; } }
		public static string AnalysisBand { get { return "Analysis Band"; } }
		#endregion

	}
}
