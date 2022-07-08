namespace Sdl.Community.MTCloud.Provider.Events
{
	public class TranslationProviderRateItOptionsChanged
	{
		public TranslationProviderRateItOptionsChanged(bool sendFeedback)
		{
			SendFeedback = sendFeedback;
		}

		public bool SendFeedback { get; }
	}
}