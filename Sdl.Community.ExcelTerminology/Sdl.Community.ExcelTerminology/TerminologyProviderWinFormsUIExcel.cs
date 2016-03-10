using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.ExcelTerminology.Insights;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Community.ExcelTerminology.Ui;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology
{
    [TerminologyProviderWinFormsUI]
    public class TerminologyProviderWinFormsUIExcel: ITerminologyProviderWinFormsUI
    {
        
        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return true;
        }

        public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
        {
            var result = new List<ITerminologyProvider>();
            try
            {
                TelemetryService.Instance.Init();
                var settingsDialog = new Settings();
                var dialogResult = settingsDialog.ShowDialog();

                if (dialogResult == DialogResult.OK ||
                    dialogResult == DialogResult.Yes)
                {
                    var settings = settingsDialog.GetSettings();

                    var persistenceService = new PersistenceService();

                    var provider = new TerminologyProviderExcel(settings);
                    settings.Uri = provider.Uri;
                    persistenceService.AddSettings(settings);
                    var providerSettings = persistenceService.Load(provider.Uri);
                    var termSearchService = new NormalTermSeachService(providerSettings);

                    var excelProvider = new TerminologyProviderExcel(providerSettings, termSearchService);

                    result.Add(excelProvider);
                }
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
                throw;
            }

            return result.ToArray();
        }

        public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
        {
            return true;
        }

        public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
        {
            return new TerminologyProviderDisplayInfo
            {
                Name = "Excel",
                TooltipText = "excel"
            };
        }

        public string TypeName => PluginResources.ExcelTerminologyProviderName;
        public string TypeDescription => PluginResources.ExcelTerminologyProviderDescription;
        public bool SupportsEditing => true;
    }
}
