using Autofac;
using InterpretBank.Model;
using InterpretBank.SettingsService.UI;
using InterpretBank.Studio.TellMe.WarningWindow;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace InterpretBank.Studio.TellMe.Actions
{
    public class SettingsAction : InterpretBankTellMeAction
    {
        public SettingsAction()
        {
            Name = "InterpretBank Settings";
            Keywords = ["interpret", "bank", "settings", "interpretbank"];
        }

        private ILifetimeScope SettingsActionLifetimeScope { get; } = ApplicationInitializer.ApplicationLifetimeScope.BeginLifetimeScope();

        public override Icon Icon => PluginResources.Settings;

        public override void Execute()
        {
            var persistenceService = SettingsActionLifetimeScope.Resolve<PersistenceService.PersistenceService>();
            var settings = persistenceService.GetSettingsForSelectedProject();
            var settingsUi = SettingsActionLifetimeScope.Resolve<SettingsMain>();

            if (settings == null)
            {
                ShowWarningWindow(); 
                return;
            }

            settingsUi.Setup(settings);
            if (!(settingsUi.ShowDialog() ?? false)) return;
            settings = settingsUi.Settings;
            settings.SettingsId = persistenceService.GetSettingsId();

            persistenceService.SaveSettings(settingsUi.Settings, settings.SettingsId);
        }

        private void ShowWarningWindow() => new SettingsActionWarning("https://appstore.rws.com/Plugin/243?tab=documentation").ShowDialog();
    }
}