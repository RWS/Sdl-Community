using Autofac;
using InterpretBank.Booth;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using System;
using System.Windows.Forms.Integration;

namespace InterpretBank.Studio.Actions
{
    [Action(nameof(StartBoothWindowAction), Icon = "Booth", Name = "In the booth",
        Description = "Start Booth Window")]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class StartBoothWindowAction : AbstractAction
    {
        protected override void Execute()
        {
            var boothWindow = ApplicationInitializer.ApplicationLifetimeScope.Resolve<BoothWindow>();
            ElementHost.EnableModelessKeyboardInterop(boothWindow);
            boothWindow.Show();
        }
    }
}