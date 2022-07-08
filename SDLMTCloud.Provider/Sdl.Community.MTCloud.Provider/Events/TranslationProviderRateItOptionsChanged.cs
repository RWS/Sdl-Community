namespace Sdl.Community.MTCloud.Provider.Events
{
	public class TranslationProviderRateItOptionsChanged
	{
		public bool SendFeedback { get; }

		public TranslationProviderRateItOptionsChanged(bool sendFeedback)
		{
			SendFeedback = sendFeedback;
		}
	}
}
