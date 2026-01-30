using Sdl.Community.NumberVerifier.Validator;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Model
{
	public class ErrorReporting
	{
		public ErrorLevel ErrorLevel { get; set; }
		public string ErrorMessage { get; set; }
		public NumberText.ErrorLevel ErrorType { get; set; }
		public string ExtendedErrorMessage { get; set; }

		public string InitialSourceNumber { get; set; }
		public string InitialTargetNumber { get; set; }
		public bool IsHindiVerification { get; set; }
		public string SourceNumberIssues { get; set; }
		public string TargetNumberIssues { get; set; }
		public string Text { get; set; }
	}
}