using Autofac;
using InterpretBank.Booth;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using System;

namespace InterpretBank.Studio.Actions
{
    [Action(nameof(StartBoothWindowAction), Icon = "Booth.ico", Name = "Booth",
        Description = "Start Booth Window")]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class StartBoothWindowAction : AbstractAction
    {
        protected override void Execute()
        {
            var boothWindow = ApplicationInitializer.ApplicationLifetimeScope.Resolve<BoothWindow>();
            boothWindow.Show();
        }
    }
}