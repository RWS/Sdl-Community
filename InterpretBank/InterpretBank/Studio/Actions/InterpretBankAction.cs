using InterpretBank.CommonServices;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.UI;
using InterpretBank.SettingsService.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using ExchangeService = InterpretBank.GlossaryExchangeService.GlossaryExchangeService;

namespace InterpretBank.Studio.Actions
{
    [Action("InterpretBankAction", Icon = "logo", Name = "Database settings",
        Description = "InterpretBank terminology provider database settings")]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class InterpretBankAction : AbstractAction
    {
        private static ExchangeService _glossaryExchangeService;

        private static GlossarySetupViewModel _glossarySetupViewModel;

        private static ExchangeService GlossaryExchangeService { get; } = _glossaryExchangeService ??= new ExchangeService();

        private static GlossarySetup GlossarySetup => new(GlossarySetupViewModel);

        private static GlossarySetupViewModel GlossarySetupViewModel { get; } = _glossarySetupViewModel ??=
            new GlossarySetupViewModel(UserInteractionService.Instance, GlossaryExchangeService, InterpretBankDataContext);

        private static IInterpretBankDataContext InterpretBankDataContext => new InterpretBankDataContext();

        public static void ShowGlossarySetup()
        {
            GlossarySetupViewModel.Setup();
            GlossarySetup.ShowDialog();
            InterpretBankDataContext.Dispose();
        }

        protected override void Execute() => ShowGlossarySetup();
    }
}