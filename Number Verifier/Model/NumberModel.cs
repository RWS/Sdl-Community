using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Model
{
	public class NumberModel
	{
		public string SourceText { get; set; }
		public string TargetText { get; set; }
		public string TargetArabicText { get; set; }
		public string SourceArabicText { get; set; }

		public INumberVerifierSettings Settings { get; set; }
		public List<string> SourceNumbers { get; set; }
		public List<string> TargetNumbers { get; set; }
		public List<string> InitialSourceNumbers { get; set; }
		public List<string> InitialTargetNumbers { get; set; }
		public bool IsHindiVerification { get; set; }
	}
}