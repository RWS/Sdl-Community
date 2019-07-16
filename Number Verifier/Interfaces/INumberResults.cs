using System.Collections.Generic;

namespace Sdl.Community.NumberVerifier.Interfaces
{
	public interface INumberResults
    {
        INumberVerifierSettings Settings { get; set; }
        List<string> SourceNumbers { get; set; }
        List<string> TargetNumbers { get; set; }
		List<string> InitialSourceNumbers { get; set; }
		List<string> InitialTargetNumbers { get; set; }
		string SourceText { get; set; }
        string  TargetText { get; set; }
		bool IsHindiVerification { get; set; }
    }
}