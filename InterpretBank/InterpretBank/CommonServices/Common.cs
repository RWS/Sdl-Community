using InterpretBank.CommonServices;

namespace InterpretBank.Service
{
    public static class Common
    {
        private static DialogService _dialogService;
        public static DialogService DialogService { get; } = _dialogService ??= new DialogService();
    }
}