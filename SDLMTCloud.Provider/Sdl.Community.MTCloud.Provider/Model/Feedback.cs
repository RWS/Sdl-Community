namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Feedback
	{
		public Feedback(string originalTarget, string originalSource)
		{
			OriginalMtCloudTranslation = originalTarget;
			OriginalSource = originalSource;
		}

		public string Suggestion { get; set; }

		public string OriginalMtCloudTranslation { get; set; }

		public string OriginalSource { get; }
	}
}