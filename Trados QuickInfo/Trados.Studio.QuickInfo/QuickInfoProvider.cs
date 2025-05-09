using Sdl.TellMe.ProviderApi;

namespace TradosStudioQuickInfo
{
    [TellMeSearchProvider]
    public class QuickInfoProvider : ITellMePluginLoader
    {
        public ISearchDataProvider InitializeProvider()
        {
            var quickInfoSearchDataProvider = new QuickInfoSearchDataProvider();
            return quickInfoSearchDataProvider;
        }
    }
}
