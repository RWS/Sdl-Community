using System;
using System.Text.RegularExpressions;

namespace Multilingual.XML.FileType.Models
{
	public class TimeSpanInfo
	{
		public TimeSpan TimeSpanStart { get; set; }
		public TimeSpan TimeSpanEnd { get; set; }

		public Match MatchStart { get; set; }
		public Match MatchEnd { get; set; }

		public string TimeSpanText { get; set; }

	}
}
