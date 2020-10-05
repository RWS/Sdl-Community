namespace Sdl.Community.MTCloud.Provider.Model
{
	public class Feedback
	{
		public Feedback(string originalSource)
		{
			OriginalSource = originalSource;
		}

		public string ActualTranslation { get; set; }

		public string OriginalMtCloudTranslation { get; set; }

		public string OriginalSource { get; }
	}
}