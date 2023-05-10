using System;
using System.Windows.Forms;
using InterpretBank.SettingsService.UI;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio;

[TerminologyProviderWinFormsUI]
internal class InterpretBankWinFormsUI : ITerminologyProviderWinFormsUI
{
	public bool SupportsEditing => true;

	public string TypeDescription => PluginResources.Plugin_Description;

	public string TypeName => PluginResources.Plugin_Name;

	public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
	{
		var provider = InterpretBankProviderFactory.GetInterpretBankProvider();

		var settingsUi = new SettingsMain { DataContext = provider.SettingsService };

		if (settingsUi.ShowDialog() ?? false)
			return new ITerminologyProvider[] { provider };

		return null;
	}

	public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
	{
		return true;

		//return dialogResult
	}

	public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
	{
		return new TerminologyProviderDisplayInfo { Name = PluginResources.Plugin_Description };
	}

	public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
	{
		return terminologyProviderUri == new Uri(Constants.InterpretBankUri);
	}
}