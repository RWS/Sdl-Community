using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Validator
{
	public class NumberFormattingSettings
	{
		public List<string> DecimalSeparators { get; set; }
		public bool OmitLeadingZero { get; set; }
		public List<string> NumberAreaSeparators { get; set; }
		public List<string> ThousandSeparators { get; set; }
		public List<ExcludedRange> ExcludedRanges { get; set; }
	}
}