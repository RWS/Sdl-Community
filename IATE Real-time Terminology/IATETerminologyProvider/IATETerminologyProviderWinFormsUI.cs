using Sdl.Community.IATETerminologyProvider.Behaviours;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Service;
using System;
using TradosStudio.API.TranslationResources.Terminology;
using TradosStudio.API.TranslationResources.Terminology.Behaviours.Interfaces;

namespace Sdl.Community.IATETerminologyProvider
{
    public class IATETerminologyProviderWinFormsUI : ITerminologyProviderWinFormsUI
    {
        private ICacheProvider _cacheProvider;
        private IConnectionProvider _connectionProvider;

        public string TypeDescription => PluginResources.IATETerminologyProviderDescription;
        public string TypeName => PluginResources.IATETerminologyProviderName;

        public IATETerminologyProviderWinFormsUI(ICacheProvider cacheProvider, IConnectionProvider connectionProvider)
        {
            _cacheProvider = cacheProvider;
            _connectionProvider = connectionProvider;
        }

        public T GetBehaviour<T>() where T : ITerminologyProviderBehaviour
        {
            if(typeof(T) == typeof(IEditBehaviour))
            {
                return (T)(ITerminologyProviderBehaviour)new IATEEditBehaviour(_cacheProvider, _connectionProvider);
            }

            if (typeof(T) == typeof(IBrowseBehaviour))
            {
                return (T)(ITerminologyProviderBehaviour)new IATEIBrowseBehaviour(_cacheProvider, _connectionProvider);
            }

            return default;
        }

        public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
        {
            return new TerminologyProviderDisplayInfo
            {
                Name = Constants.IATEProviderName,
                TooltipText = Constants.IATEProviderDescription
            };
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return terminologyProviderUri.Scheme == Constants.IATEGlossary;
        }
    }
}