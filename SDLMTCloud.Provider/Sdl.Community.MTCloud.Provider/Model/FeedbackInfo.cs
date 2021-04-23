using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class FeedbackInfo
	{
		public QualityEstimation Evaluation { get; set; }
		public string OriginalMtCloudTranslation { get; set; }
		public dynamic Rating { get; set; }
		public SegmentId? SegmentId { get; set; }
		public string Suggestion { get; set; }
	}
}