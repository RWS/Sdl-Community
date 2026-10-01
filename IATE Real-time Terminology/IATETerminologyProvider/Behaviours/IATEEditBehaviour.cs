using NLog;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.View;
using System;
using System.Windows.Forms;
using TradosStudio.API.TranslationResources.Terminology;
using TradosStudio.API.TranslationResources.Terminology.Behaviours.Interfaces;

namespace Sdl.Community.IATETerminologyProvider.Behaviours
{
    public class IATEEditBehaviour : IEditBehaviour
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private MainWindow _mainWindow;

        private ICacheProvider _cacheProvider;
        private IConnectionProvider _connectionProvider;

        public IATEEditBehaviour(ICacheProvider cacheProvider, IConnectionProvider connectionProvider)
        {
            _cacheProvider = cacheProvider;
            _connectionProvider = connectionProvider;
        }


        public bool Edit(IntPtr owner, ITerminologyProvider terminologyProvider)
        {
            if (!_connectionProvider.EnsureConnection())
            {
                var exception = new Exception("Failed login!");
                _logger.Error(exception);

                throw exception;
            }

            var provider = terminologyProvider as IATETerminologyProvider;
            if (provider == null)
            {
                return false;
            }

            _mainWindow = IATEApplication.GetMainWindow(_cacheProvider, _connectionProvider);

            if (!_mainWindow.ShowDialog() ?? false)
            {
                return false;
            }

            provider.ProviderSettings = _mainWindow.ProviderSettings;

            return true;
        }
    }
}
