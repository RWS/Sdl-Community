using Sdl.FileTypeSupport.Framework.NativeApi;

namespace LanguageWeaverProvider.Model
{
	public class RatedSegment
	{
		public bool AutosendFeedback { get; set; }

		public string QualityEstimation { get; set; }

		public string Model { get; set; }

		public string ModelName { get; set; }

		public string Translation { get; set; }

		public SegmentId SegmentId { get; set; }

		public string TargetLanguageCode { get; set; }

        //TODO: Developer named variable FileName, but it stores the FilePath; needs to be revised to avoid confusion
        public string FileName { get; set; }
	}
}