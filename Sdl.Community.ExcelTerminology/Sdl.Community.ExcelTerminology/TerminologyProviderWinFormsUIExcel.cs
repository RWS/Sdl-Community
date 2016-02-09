using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
           
            var settingsDialog = new Settings();
            settingsDialog.ShowDialog();
            var persistenceService = new PersistenceService();
            var termSearchService = new NormalTermSeachService();
            var providerSettings = persistenceService.Load();
            var excelProvider = new TerminologyProviderExcel(providerSettings, termSearchService);

            return new ITerminologyProvider[] { excelProvider };
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
