using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.Verification.Api;

namespace Multilingual.Excel.FileType.Verifier.MessageUI
{
	public class MultilingualExcelMessageData : ExtendedMessageEventData, IVerificationCustomMessageData
	{
		public MultilingualExcelMessageData(string message)
		{
			DetailedDescription = message;
			MessageType = "MultilingualExcel.MessageUI";
		}

		public string DetailedDescription { get; }
		public string SourceSegmentPlainText { get; set; }
		public string TargetSegmentPlainText { get; set; }
	}
}
