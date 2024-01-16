using InterpretBank.GlossaryService.Interface;

namespace InterpretBank.SettingsService.ViewModel.Interface
{
	public interface IGlossarySetupViewModel : ISubViewModel
	{
		public void SetDataContext(IInterpretBankDataContext interpretBankDataContext);
	}
}