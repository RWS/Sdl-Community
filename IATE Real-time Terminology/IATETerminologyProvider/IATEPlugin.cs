using Sdl.Community.IATETerminologyProvider.IATEProviderTellMe;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.TellMe.ProviderApi;
using TradosStudio.API;
using TradosStudio.API.TranslationResources.Terminology;

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
            container.Register<ITellMeProvider, IATETellMeProvider>(Lifestyle.Singleton);
            container.AppendCollection<ITerminologyProviderFactory, IATETerminologyProviderFactory>();
            container.AppendCollection<ITerminologyProviderViewerWinFormsUI, IATETerminologyProviderViewerWinFormsUI>();           
            container.AppendCollection<ITerminologyProviderWinFormsUI, IATETerminologyProviderWinFormsUI>();
        }
    }
}
