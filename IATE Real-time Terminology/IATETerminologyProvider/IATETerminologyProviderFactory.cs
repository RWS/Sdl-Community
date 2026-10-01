using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using TradosStudio.API.TranslationResources.Terminology;

namespace Sdl.Community.IATETerminologyProvider
{
    public class IATETerminologyProviderFactory : ITerminologyProviderFactory
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private ICacheProvider _cacheProvider;

        public IATETerminologyProviderFactory(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return terminologyProviderUri.Scheme == Constants.IATEGlossary;
        }

        public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri)
        {
            return CreateTerminologyProvider();
        }

        private ITerminologyProvider CreateTerminologyProvider()
        {
            var savedSettings = SettingsService.GetSettingsForCurrentProject();
            var savedTermTypesNumber = savedSettings?.TermTypes.Count;

            if(!IATEApplication.IsInitialized)
            {
                System.Threading.Tasks.Task.Run(async () => await IATEApplication.ExecuteAsync()).GetAwaiter().GetResult();
            }

            if (savedTermTypesNumber > 0 && savedTermTypesNumber > IATEApplication.InventoriesProvider.TermTypes?.Count)
            {
                var availableTermTypes = GetAvailableTermTypes(savedSettings.TermTypes);
                savedSettings.TermTypes = new List<TermTypeModel>(availableTermTypes);
            }

            if (!IATEApplication.ConnectionProvider.EnsureConnection())
            {
                var exception = new Exception("Failed login!");
                _logger.Error(exception);

                throw exception;
            }

            var terminologyProvider = new IATETerminologyProvider(
                savedSettings,
                IATEApplication.ConnectionProvider, 
                IATEApplication.InventoriesProvider, 
                _cacheProvider);

            return terminologyProvider;
        }

        private List<TermTypeModel> GetAvailableTermTypes(List<TermTypeModel> savedList)
        {
            var availableTerms = savedList.Where(t =>
                IATEApplication.InventoriesProvider.TermTypes.Any(t1 => t1.Code == t.Code.ToString())).ToList();

            return availableTerms;
        }
    }
}