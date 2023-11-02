using InterpretBank.GlossaryService;
using InterpretBank.SettingsService.UI;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using IWin32Window = System.Windows.Forms.IWin32Window;
using Settings = InterpretBank.SettingsService.Settings;

namespace InterpretBank.Studio;

[TerminologyProviderWinFormsUI]
internal class InterpretBankWinFormsUI : ITerminologyProviderWinFormsUI
{
    public bool SupportsEditing => true;

    public string TypeDescription => PluginResources.Plugin_Description;

    public string TypeName => PluginResources.Plugin_Name;

    public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
    {
        var interpretBankDataContext = new InterpretBankDataContext();
        var settingsService =
            new SettingsService.ViewModel.SettingsService(interpretBankDataContext);
        var settingsUi = new SettingsMain { DataContext = settingsService };

        Settings settings;
        if (settingsUi.ShowDialog() ?? false)
            settings = settingsService.Settings;
        else
            return null;

        var settingsId = GetSettingsId();
        settings.SettingsId = settingsId;

        PersistenceService.PersistenceService.SaveSettings(settings, settingsId);

        var termSearchService = new TerminologyService.TerminologyService(interpretBankDataContext);
        var provider = new InterpretBankProvider(termSearchService, settings);

        return new ITerminologyProvider[] { provider };
    }

    public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
    {
        var provider = terminologyProvider as InterpretBankProvider;
        if (provider == null)
            return false;

        var settingsService =
            new SettingsService.ViewModel.SettingsService(provider.TermSearchService.InterpretBankDataContext);
        settingsService.Settings = provider.Settings;

        var settingsUi = new SettingsMain { DataContext = settingsService };
        var result = settingsUi.ShowDialog() ?? false;

        if (!result)
            return false;

        provider.Settings = settingsService.Settings;
        PersistenceService.PersistenceService.SaveSettings(settingsService.Settings, provider.Settings.SettingsId);

        provider.RaiseProviderSettingsChanged();
        return true;
    }

    public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri)
    {
        return new TerminologyProviderDisplayInfo { Name = PluginResources.Plugin_Description };
    }

    public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri) =>
        terminologyProviderUri.ToString().Contains(Constants.InterpretBankUri);

    private static string GetSettingsId() => (Guid.NewGuid().ToString());
}