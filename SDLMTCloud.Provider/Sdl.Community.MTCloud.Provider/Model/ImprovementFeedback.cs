namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ImprovementFeedback
	{
		public ImprovementFeedback(string originalTarget, string originalSource)
		{
			OriginalMtCloudTranslation = originalTarget;
			OriginalSource = originalSource;
		}

		public string Improvement { get; set; }

		public string OriginalMtCloudTranslation { get; set; }

		public string OriginalSource { get; }
	}
}