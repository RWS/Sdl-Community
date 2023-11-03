using Autofac;
using InterpretBank.SettingsService.UI;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using IWin32Window = System.Windows.Forms.IWin32Window;

namespace InterpretBank.Studio;

[TerminologyProviderWinFormsUI]
internal class InterpretBankWinFormsUI : ITerminologyProviderWinFormsUI
{
    public InterpretBankWinFormsUI()
    {
        
    }
    public bool SupportsEditing => true;

    public string TypeDescription => PluginResources.Plugin_Description;

    public string TypeName => PluginResources.Plugin_Name;

    private ILifetimeScope InterpretBankSettingsScope { get; } = ApplicationInitializer.Container.BeginLifetimeScope();

    private PersistenceService.PersistenceService PersistenceService { get; } = ApplicationInitializer.Container.Resolve<PersistenceService.PersistenceService>();

    public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
    {
        var settingsUi = InterpretBankSettingsScope.Resolve<SettingsMain>();

        if (!(settingsUi.ShowDialog() ?? false))
            return null;

        var settings = settingsUi.Settings;

        var settingsId = GetSettingsId();
        settings.SettingsId = settingsId;

        PersistenceService.SaveSettings(settings, settingsId);

        var provider = InterpretBankSettingsScope.Resolve<InterpretBankProvider>();
        provider.Setup(settings);

        return new ITerminologyProvider[] { provider };
    }

    public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
    {
        var provider = terminologyProvider as InterpretBankProvider;
        if (provider == null)
            return false;

        //var settingsService =
        //    new SettingsService.ViewModel.SettingsService(provider.TermSearchService.InterpretBankDataContext);
        //settingsService.Settings = provider.Settings;

        //var settingsUi = new SettingsMain(settingsService);

        var settingsUi = InterpretBankSettingsScope.Resolve<SettingsMain>();
        settingsUi.Setup(provider.Settings);
        var result = settingsUi.ShowDialog() ?? false;

        if (!result)
            return false;

        provider.Settings = settingsUi.Settings;
        PersistenceService.SaveSettings(provider.Settings, provider.Settings.SettingsId);

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