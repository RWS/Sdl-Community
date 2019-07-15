using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Model
{
	public class NumberResults : INumberResults
    {
		public NumberResults(INumberVerifierSettings settings, List<string> sourceNumbers, List<string> targetNumbers)
        {
            Settings = settings;
            SourceNumbers = sourceNumbers;
            TargetNumbers = targetNumbers;
        }

        public NumberResults(
			INumberVerifierSettings settings, 
			List<string> sourceNumbers,
			List<string> targetNumbers,
			List<string> initialSourceNumbers,
			List<string> initialTargetNumbers,
			string sourceText,
			string targetText) 
            :this(settings, sourceNumbers,targetNumbers)
        {
            SourceText = sourceText;
			TargetText = targetText;
			InitialSourceNumbers = initialSourceNumbers;
			InitialTargetNumbers = initialTargetNumbers;
		}

		public INumberVerifierSettings Settings { get; set; }

        public List<string> SourceNumbers { get; set; }

        public List<string> TargetNumbers { get; set; }

		public List<string> InitialSourceNumbers { get; set; }

        public List<string> InitialTargetNumbers { get; set; }

        public string SourceText { get; set; }

        public string TargetText { get; set; }
    }
}