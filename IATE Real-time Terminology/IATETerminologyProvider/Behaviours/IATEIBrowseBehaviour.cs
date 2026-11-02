using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.View;
using System;
using System.Collections.Generic;
using System.Linq;
using TradosStudio.API.ProjectManagement;
using TradosStudio.API.TranslationResources.Terminology;
using TradosStudio.API.TranslationResources.Terminology.Behaviours.Interfaces;

namespace Sdl.Community.IATETerminologyProvider.Behaviours
{
    public class IATEIBrowseBehaviour : IBrowseBehaviour
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private MainWindow _mainWindow;

        private ICacheProvider _cacheProvider;
        private IConnectionProvider _connectionProvider;
        private IInventoriesProvider _inventoriesProvider;
        private ITerminologyProviderFactory _treminologyProviderFactory;
        private IProjectsRegistry _projectsRegistry;

        public IATEIBrowseBehaviour(
            ICacheProvider cacheProvider, 
            IConnectionProvider connectionProvider, 
            IInventoriesProvider inventoriesProvider,
            ITerminologyProviderFactory terminologyProviderFactory,
            IProjectsRegistry projectsRegistry)
        {
            _cacheProvider = cacheProvider;
            _connectionProvider = connectionProvider;
            _inventoriesProvider = inventoriesProvider;
            _treminologyProviderFactory = terminologyProviderFactory;
            _projectsRegistry = projectsRegistry;
        }

        public IEnumerable<ITerminologyProvider> Browse(IntPtr owner)
        {
            return Browse().AsEnumerable();
        }

        private ITerminologyProvider[] Browse()
        {
            _mainWindow = IATEApplication.GetMainWindow(
                _cacheProvider, 
                _connectionProvider, 
                _inventoriesProvider, 
                _projectsRegistry);

            if (_mainWindow != null)
            {
                _mainWindow.ShowDialog();
                if (!_mainWindow?.DialogResult ?? true)
                {
                    return null;
                }

                var provider = (IATETerminologyProvider)_treminologyProviderFactory.CreateTerminologyProvider(new Uri(Constants.IATEUriTemplate));                 
                provider.ProviderSettings = _mainWindow.ProviderSettings;

                return new ITerminologyProvider[] { provider };
            }

            var exception = new Exception("Failed login!");
            _logger.Error(exception);

            throw exception;
        }
    }
}
