using Autofac;
using InterpretBank.Model;
using InterpretBank.SettingsService.UI;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace InterpretBank.Studio.TellMe
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

            var settings = persistenceService.GetSettingsForCurrentProject();

            var settingsUi = SettingsActionLifetimeScope.Resolve<SettingsMain>();
            settingsUi.Setup(settings);
            settingsUi.ShowDialog();

            persistenceService.SaveSettings(settingsUi.Settings, settings.SettingsId);
        }
    }
}