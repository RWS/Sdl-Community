using InterpretBank.GlossaryService;
using InterpretBank.SettingsService.UI;
using InterpretBank.SettingsService.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;

namespace InterpretBank.Studio
{
    [Action("InterpretBankAction", Icon = "logo", Name = "InterpretBank database settings",
        Description = "InterpretBank terminology provider database settings")]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class InterpretBankAction : AbstractAction
    {
        protected override void Execute() => ServiceManager.GlossarySetup.ShowDialog();
    }

    [RibbonGroup("InterpretBankRibbonGroup", Name = "InterpretBank")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class InterpretBankRibbonGroup : AbstractRibbonGroup
    {
    }
}