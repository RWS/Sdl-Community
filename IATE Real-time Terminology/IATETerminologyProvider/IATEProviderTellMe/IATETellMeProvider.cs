using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.TellMe.ProviderApi;
using TradosStudio.API.ProjectManagement;

namespace Sdl.Community.IATETerminologyProvider.IATEProviderTellMe
{
	[TellMeProvider]
	public class IATETellMeProvider : ITellMeProvider
	{
        private ICacheProvider _cacheProvider;
        private IConnectionProvider _connectionProvider;
        private IInventoriesProvider _inventoriesProvider;
        private IProjectsRegistry _projectsRegistry;

        public IATETellMeProvider(
            ICacheProvider cacheProvider, 
            IConnectionProvider connectionProvider, 
            IInventoriesProvider inventoriesProvider,
            IProjectsRegistry projectsRegistry)
        {
            _cacheProvider = cacheProvider;
            _connectionProvider = connectionProvider;
            _inventoriesProvider = inventoriesProvider;
            _projectsRegistry = projectsRegistry;
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
			}, new IATESettingsAction(_cacheProvider, _connectionProvider, _inventoriesProvider, _projectsRegistry)
			{
				Keywords = ["iate", "settings"]
			}
		];
	}
}