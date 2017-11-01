using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.DisplayFilters
{
	public class CustomFilterSettings
	{
		public bool OddsNo { get; set; }
		public bool EvenNo { get; set; }
		public bool CommaSeparated { get; set; }
		public bool Grouped { get; set; }
		public string CommaSeparatedVelues { get; set; }
		public string GroupedList { get; set; }
		public bool UseRegexCommentSearch { get; set; }
		public string CommentRegex { get; set; }
	}
}
