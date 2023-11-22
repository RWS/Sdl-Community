using System;
using System.Windows.Forms;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.View;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider
{
	[TerminologyProviderWinFormsUI]
    public class IATETerminologyProviderWinFormsUI : ITerminologyProviderWinFormsUIWithEdit
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private MainWindow _mainWindow;

        public string TypeDescription => PluginResources.IATETerminologyProviderDescription;
        public string TypeName => PluginResources.IATETerminologyProviderName;

        public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
        {
            _mainWindow = IATEApplication.GetMainWindow();
            if (_mainWindow != null)
            {
				_mainWindow.ShowDialog();
				if (!_mainWindow?.DialogResult ?? true)
                {
                    return null;
                }

                var provider = new IATETerminologyProvider(_mainWindow.ProviderSettings, IATEApplication.ConnectionProvider,
                    IATEApplication.InventoriesProvider, IATEApplication.CacheProvider,IATEApplication.EUProvider);

                return new ITerminologyProvider[] { provider };
            }

            var exception = new Exception("Failed login!");
            _logger.Error(exception);

            throw exception;
        }

        public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
        {
            if (!IATEApplication.ConnectionProvider.EnsureConnection())
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

            _mainWindow = IATEApplication.GetMainWindow();

            if (!_mainWindow.ShowDialog() ?? false)
            {
                return false;
            }

            provider.ProviderSettings = _mainWindow.ProviderSettings;

            return true;
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