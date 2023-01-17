﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.TermExcelerator.Services;
using Sdl.Community.TermExcelerator.Ui;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator
{
	[TerminologyProviderWinFormsUI]
	public class TerminologyProviderWinFormsUIExcel : ITerminologyProviderWinFormsUI
	{
		public string TypeName => PluginResources.ExcelTerminologyProviderName;
		public string TypeDescription => PluginResources.ExcelTerminologyProviderDescription;
		public bool SupportsEditing => true;
		public static readonly Log Log = Log.Instance;

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "excelglossary";
		}

		public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
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
				Log.Logger.Error($"Browse method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
			return result.ToArray();
		}

		public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
		{
			if (terminologyProvider is not TerminologyProviderExcel provider)
			{
				return false;
			}

			var settingsDialog = new Settings();
			settingsDialog.SetSettings(provider.ProviderSettings);
			var dialogResult = settingsDialog.ShowDialog();
			if (dialogResult == DialogResult.OK ||
				dialogResult == DialogResult.Yes)
			{
				provider.ProviderSettings = settingsDialog.GetSettings();
			}

			return true;
		}

		public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
		{
			var name = terminologyProviderUri.AbsolutePath.Replace(TerminologyProviderExcel.ExcelUriTemplate, "").Replace("/", "");
			return new TerminologyProviderDisplayInfo
			{
				Name = name,
				TooltipText = name
			};
		}
	}
}