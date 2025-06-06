﻿using Autofac;
using InterpretBank.SettingsService.UI;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Windows.Forms;

namespace InterpretBank.Studio
{
    [TerminologyProviderWinFormsUI]
    internal class InterpretBankWinFormsUI : ITerminologyProviderWinFormsUIWithEdit
    {
        public string TypeDescription => PluginResources.Plugin_Description;

        public string TypeName => PluginResources.Plugin_Name;

        private ILifetimeScope InterpretBankSettingsScope { get; } = ApplicationInitializer.ApplicationLifetimeScope.BeginLifetimeScope();

        private PersistenceService.PersistenceService PersistenceService { get; } = ApplicationInitializer.ApplicationLifetimeScope.Resolve<PersistenceService.PersistenceService>();


        public ITerminologyProvider[] Browse(IWin32Window owner, ITerminologyProviderCredentialStore credentialStore)
        {
            return Browse(owner);
        }

        public ITerminologyProvider[] Browse(IWin32Window owner)
        {
            var settingsUi = InterpretBankSettingsScope.Resolve<SettingsMain>();

            if (!(settingsUi.ShowDialog() ?? false)) return null;

            var settings = settingsUi.Settings;

            var settingsId = GetSettingsId();
            settings.SettingsId = settingsId;

            PersistenceService.SaveSettings(settings, settingsId);

            var provider = InterpretBankSettingsScope.Resolve<InterpretBankProvider>();
            provider.Setup(settings);

            return [provider];
        }

        public bool Edit(IWin32Window owner, ITerminologyProvider terminologyProvider)
        {
            if (terminologyProvider is not InterpretBankProvider provider)
                return false;

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

        public TerminologyProviderDisplayInfo GetDisplayInfo(Uri terminologyProviderUri) =>
            new() { Name = PluginResources.Plugin_Description };

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri) =>
            terminologyProviderUri.ToString().Contains(Constants.InterpretBankUri);

        private static string GetSettingsId() => (Guid.NewGuid().ToString());
    }
}