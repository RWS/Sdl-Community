using Sdl.Community.NumberVerifier.Validator;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.NumberVerifier.MessageUI
{
	public class AlignmentErrorExtendedData : ExtendedMessageEventData
	{
		public TextRange SourceRange { get; set; } = new();
		public TextRange TargetRange { get; set; } = new();
		public string SourceIssues { get; set; }
		public string TargetIssues { get; set; }
	}
}