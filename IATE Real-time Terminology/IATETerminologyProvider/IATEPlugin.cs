using Sdl.Community.IATETerminologyProvider.Actions;
using Sdl.Community.IATETerminologyProvider.IATEProviderTellMe;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.View;
using Sdl.TellMe.ProviderApi;
using TradosStudio.API;
using TradosStudio.API.TranslationResources.Terminology;
using TradosStudio.API.UI;
using TradosStudio.API.UI.Action;
using TradosStudio.API.UI.View;

namespace Sdl.Community.IATETerminologyProvider
{
    public class IATEPlugin : IPlugin
    {
        public void Initialize()
        {            
        }

        public void RegisterTypes(IContainer container)
        {            
            container.Register<IMessageBoxService, MessageBoxService>(Lifestyle.Singleton);
            container.Register<IPathInfo, PathInfo>(Lifestyle.Singleton);
            container.Register<ISqliteDatabaseProvider, SqliteDatabaseProvider>(Lifestyle.Singleton);
            container.Register<ICacheProvider, CacheProvider>(Lifestyle.Singleton);
            container.Register<IConnectionProvider, ConnectionProvider>(Lifestyle.Singleton);
            container.Register<IInventoriesProvider, InventoriesProvider>(Lifestyle.Singleton);
            container.Register<ITellMeProvider, IATETellMeProvider>(Lifestyle.Singleton);
            container.AppendCollection<IRibbonGroup, IATETerminologyProviderRibbonGroup>();
            container.AppendCollection<IActionMetaData, IATESearchSourceLanguageActionMetaData>();
            container.AppendCollection<IActionMetaData, IATESearchAllActionMetaData>();
            container.AppendCollection<IAction, IATESearchSourceLanguageAction>();
            container.AppendCollection<IAction, IATESearchAllAction>();
            container.AppendCollection<IViewPart, SearchResultsViewPart>();
            container.AppendCollection<IViewPartMetaData, SearchResultsViewPartMetaData>();
            container.AppendCollection<ITerminologyProviderFactory, IATETerminologyProviderFactory>();
            container.AppendCollection<ITerminologyProviderViewerWinFormsUI, IATETerminologyProviderViewerWinFormsUI>();           
            container.AppendCollection<ITerminologyProviderWinFormsUI, IATETerminologyProviderWinFormsUI>();
        }
    }
}
