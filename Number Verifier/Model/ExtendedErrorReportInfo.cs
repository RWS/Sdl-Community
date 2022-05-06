using Sdl.Community.Extended.MessageUI;
using Sdl.Community.NumberVerifier.MessageUI;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.NumberVerifier.Model
{
	public class ExtendedErrorReportInfo
	{
		public TextLocation EndLocation { get; set; }
		public ErrorLevel ErrorLevel { get; set; }
		public string Message { get; set; }
		public AlignmentErrorExtendedData Report { get; set; }
		public TextLocation StartLocation { get; set; }
	}
}