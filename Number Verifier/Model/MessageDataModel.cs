using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.NumberVerifier.Model
{
	public class MessageDataModel
	{
		public string SourceIssues { get; set; }
		public string TargetIssues { get; set; }
		public ISegment ReplacementSuggestion { get; set; }
		public string InitialSourceIssues { get; set; }
		public string InitialTargetIssues { get; set; }
		public string ErrorMessage { get; set; }
		public bool IsHindiVerification { get; set; }
	}
}