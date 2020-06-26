using System.Collections.Generic;

namespace Sdl.Community.NumberVerifier.Model
{
	public class NormalizedNumber
	{
		public List<string> InitialNumberList { get; set; }
		public List<string> NormalizedNumberList { get; set; }
		public string Text { get; set; }
		public string DecimalSeparators { get; set; }
		public string ThousandSeparators { get; set; }
		public string Separators { get; set; }
		public bool OmitLeadingZero { get; set; }
		public bool IsNoSeparator { get; set; }
	}
}
