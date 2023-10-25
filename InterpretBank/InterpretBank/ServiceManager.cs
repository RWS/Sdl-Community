using InterpretBank.CommonServices;
using InterpretBank.GlossaryService;
using InterpretBank.SettingsService.UI;
using InterpretBank.SettingsService.ViewModel;
using ExchangeService = InterpretBank.GlossaryExchangeService.GlossaryExchangeService;

namespace InterpretBank
{
    public static class ServiceManager
    {
        private static UserInteractionService _dialogService;
        private static ExchangeService _glossaryExchangeService;
        private static GlossarySetupViewModel _glossarySetupViewModel;
        public static UserInteractionService DialogService { get; } = _dialogService ??= new UserInteractionService();
        public static ExchangeService GlossaryExchangeService { get; } = _glossaryExchangeService ??= new ExchangeService();

        private static GlossarySetup GlossarySetup => new(GlossarySetupViewModel);

        public static void ShowGlossarySetup()
        {
            GlossarySetupViewModel.Setup();
            GlossarySetup.ShowDialog();
        }

        private static GlossarySetupViewModel GlossarySetupViewModel { get; } = _glossarySetupViewModel ??=
                    new GlossarySetupViewModel(DialogService, GlossaryExchangeService, new InterpretBankDataContext());
    }
}