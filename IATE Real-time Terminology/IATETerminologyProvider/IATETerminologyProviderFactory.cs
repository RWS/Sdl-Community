using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using TradosStudio.API.ProjectManagement;
using TradosStudio.API.TranslationResources.Terminology;

namespace Sdl.Community.IATETerminologyProvider
{
    public class IATETerminologyProviderFactory : ITerminologyProviderFactory
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private ICacheProvider _cacheProvider;
        private IConnectionProvider _connectionProvider;
        private IInventoriesProvider _inventoriesProvider;
        private IProjectsRegistry _projectsRegistry;

        public IATETerminologyProviderFactory(
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
                System.Threading.Tasks.Task.Run(async () => await IATEApplication.ExecuteAsync(_connectionProvider, _inventoriesProvider)).GetAwaiter().GetResult();
            }

            if (savedTermTypesNumber > 0 && savedTermTypesNumber > _inventoriesProvider.TermTypes?.Count)
            {
                var availableTermTypes = GetAvailableTermTypes(savedSettings.TermTypes);
                savedSettings.TermTypes = new List<TermTypeModel>(availableTermTypes);
            }

            if (!_connectionProvider.EnsureConnection())
            {
                var exception = new Exception("Failed login!");
                _logger.Error(exception);

                throw exception;
            }

            var terminologyProvider = new IATETerminologyProvider(
                savedSettings,
                _connectionProvider, 
                _inventoriesProvider, 
                _cacheProvider,
                _projectsRegistry);

            return terminologyProvider;
        }

        private List<TermTypeModel> GetAvailableTermTypes(List<TermTypeModel> savedList)
        {
            var availableTerms = savedList.Where(t =>
                _inventoriesProvider.TermTypes.Any(t1 => t1.Code == t.Code.ToString())).ToList();

            return availableTerms;
        }
    }
}