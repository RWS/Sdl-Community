using Autofac;
using InterpretBank.SettingsService.UI;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using System;

namespace InterpretBank.Studio.Actions
{
    [Action(nameof(ConfigureDatabaseAction), Icon = "logo", Name = "Configure database",
        Description = "InterpretBank database configuration")]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class ConfigureDatabaseAction : AbstractAction
    {
        public static void ShowGlossarySetup()
        {
            ApplicationInitializer.ApplicationLifetimeScope.Resolve<GlossarySetup>().ShowDialog();
        }

        protected override void Execute() => ShowGlossarySetup();
    }
}