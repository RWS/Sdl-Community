using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.TermExcelerator.Services;
using Sdl.Community.TermExcelerator.Ui;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator
{
	[TerminologyProviderWinFormsUI]
	public class TerminologyProviderWinFormsUIExcel : ITerminologyProviderWinFormsUIWithEdit
	{
		public string TypeName => PluginResources.ExcelTerminologyProviderName;
		public string TypeDescription => PluginResources.ExcelTerminologyProviderDescription;
		public static readonly Log Log = Log.Instance;
		private PersistenceService _persistenceService;

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "excelglossary";
		}

		public PersistenceService PersistenceService => _persistenceService ??= new PersistenceService();


        public ITerminologyProvider[] Browse(IWin32Window owner)
        {
            var result = new List<ITerminologyProvider>();
            try
            {
                var settingsDialog = new Settings();
                var dialogResult = settingsDialog.ShowDialog();

                if (dialogResult == DialogResult.OK ||
                    dialogResult == DialogResult.Yes)
                {
                    var settings = settingsDialog.GetSettings();

                    var provider = new TerminologyProviderExcel(settings);
                    settings.Uri = provider.Uri;
                    PersistenceService.AddSettings(settings);
                    var providerSettings = PersistenceService.Load(provider.Uri);
                    var termSearchService = new NormalTermSeachService(providerSettings);

                    var excelProvider = new TerminologyProviderExcel(providerSettings, termSearchService);


                    result.Add(excelProvider);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"Browse method: {ex.Message}\n {ex.StackTrace}");
                throw ex;
            }
            return result.ToArray();
        }

        public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
		{
			if (!(terminologyProvider is TerminologyProviderExcel provider))
			{
				return false;
			}

			var currentSettings = provider.ProviderSettings;
			var settingsDialog = new Settings();
			settingsDialog.SetSettings(currentSettings);
			var dialogResult = settingsDialog.ShowDialog();

			if (dialogResult == DialogResult.OK ||
				dialogResult == DialogResult.Yes)
			{
				var settings = settingsDialog.GetSettings();
				settings.Uri = provider.Uri;

				PersistenceService.AddSettings(settings);
				provider.ProviderSettings = settings;
			}

			return true;
		}

		public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
		{
			var name = terminologyProviderUri.Host;
			return new TerminologyProviderDisplayInfo
			{
				Name = $"{PluginResources.Plugin_Name}: {name}",
				TooltipText = name
			};
		}
	}
}