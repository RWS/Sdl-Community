using NLog;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.View;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IATEIBrowseBehaviour(ICacheProvider cacheProvider, IConnectionProvider connectionProvider)
        {
            _cacheProvider = cacheProvider;
            _connectionProvider = connectionProvider;
        }

        public IEnumerable<ITerminologyProvider> Browse(IntPtr owner)
        {
            return Browse().AsEnumerable();
        }

        private ITerminologyProvider[] Browse()
        {
            _mainWindow = IATEApplication.GetMainWindow(_cacheProvider, _connectionProvider);
            if (_mainWindow != null)
            {
                _mainWindow.ShowDialog();
                if (!_mainWindow?.DialogResult ?? true)
                {
                    return null;
                }

                var provider = new IATETerminologyProvider(
                    _mainWindow.ProviderSettings, 
                    _connectionProvider,
                    IATEApplication.InventoriesProvider, 
                    _cacheProvider);

                return new ITerminologyProvider[] { provider };
            }

            var exception = new Exception("Failed login!");
            _logger.Error(exception);

            throw exception;
        }
    }
}
