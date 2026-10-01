using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	[TellMeProvider]
	public class IATETellMeProvider : ITellMeProvider
	{
        private ICacheProvider _cacheProvider;

        public IATETellMeProvider(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public string Name => "IATE Tell Me provider";

		public AbstractTellMeAction[] ProviderActions =>
		[
			new IATEDocumentationAction
			{
				Keywords = ["iate", "iate community", "iate support", "iate wiki"]
			},
			new IATECommunityForumAction
			{
				Keywords = ["iate", "iate community", "iate support", "iate forum"]
			},
			new IATEContactAction
			{
				Keywords = ["iate", "iate contact", "iate official", "iate website", "iate web search"]
			},new IATESourceCode
			{
				Keywords = ["iate", "source", "code"]
			}, new IATESettingsAction(_cacheProvider)
			{
				Keywords = ["iate", "settings"]
			}
		];
	}
}