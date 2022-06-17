namespace Sdl.Community.MTCloud.Provider.Model.RateIt
{
	public class FeedbackInfo
	{
		public QualityEstimation Evaluation { get; set; }
		public string OriginalMtCloudTranslation { get; set; }
		public dynamic Rating { get; set; }
		public string SegmentSource { get; set; }
		public string Suggestion { get; set; }
	}
}