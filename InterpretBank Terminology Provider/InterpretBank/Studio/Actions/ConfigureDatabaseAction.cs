using Autofac;
using InterpretBank.Events;
using InterpretBank.SettingsService.UI;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using System;

namespace InterpretBank.Studio.Actions
{
    [Action(nameof(ConfigureDatabaseAction), Icon = "IB", Name = "Configure database",
        Description = "InterpretBank database configuration")]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class ConfigureDatabaseAction : AbstractAction
    {
        protected override void Execute() => ShowGlossarySetupDialog();

        private void RaiseDbChanged(string filepath) => StudioContext.EventAggregator.Publish(new DbChangedEvent { Filepath = filepath });

        private void ShowGlossarySetupDialog()
        {
            var glossarySetup = ApplicationInitializer.ApplicationLifetimeScope.Resolve<GlossarySetup>();
            glossarySetup.ShowDialog();
            RaiseDbChanged(glossarySetup.ChooseFilepathControl.Filepath);
        }
    }
}