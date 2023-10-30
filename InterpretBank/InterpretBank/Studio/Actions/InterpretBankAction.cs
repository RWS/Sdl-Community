using InterpretBank.GlossaryService;
using InterpretBank.SettingsService.UI;
using InterpretBank.SettingsService.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using System;

namespace InterpretBank.Studio.Actions
{
    [Action("InterpretBankAction", Icon = "logo", Name = "Database settings",
        Description = "InterpretBank terminology provider database settings")]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class InterpretBankAction : AbstractAction
    {
        protected override void Execute() => ServiceManager.ShowGlossarySetup();
        
    }
}