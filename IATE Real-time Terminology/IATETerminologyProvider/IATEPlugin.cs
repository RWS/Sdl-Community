using Sdl.Community.IATETerminologyProvider.Behaviours;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;
using TradosStudio.API;
using TradosStudio.API.TranslationResources.Terminology;
using TradosStudio.API.TranslationResources.Terminology.Behaviours.Interfaces;

namespace Sdl.Community.IATETerminologyProvider
{
    public class IATEPlugin : IPlugin
    {
        public void Initialize()
        {            
        }

        public void RegisterTypes(IContainer container)
        {
            container.AppendCollection<ITerminologyProviderFactory, IATETerminologyProviderFactory>();
            container.AppendCollection<ITerminologyProviderViewerWinFormsUI, IATETerminologyProviderViewerWinFormsUI>();           
            container.AppendCollection<ITerminologyProviderWinFormsUI, IATETerminologyProviderWinFormsUI>();
        }
    }
}
