using System.Collections.Generic;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.Model;
using InterpretBank.SettingsService.ViewModel.Interface;
using InterpretBank.TerminologyService;
using InterpretBank.Wrappers.Interface;

namespace InterpretBank.SettingsService.ViewModel;

public class SettingsService : ViewModel, ISettingsService
{
	private ICommand _saveCommand;

	public SettingsService(IOpenFileDialog openFileDialog, IInterpretBankDataContext interpretBankDataContext)
	{
		InterpretBankDataContext = interpretBankDataContext;

		Tags = InterpretBankDataContext.GetTags();
		Glossaries = InterpretBankDataContext.GetGlossaries();

		ViewModels = new List<ISubViewModel> { new SettingsViewModel(Glossaries, Tags, openFileDialog), new SetupGlossariesViewModel(interpretBankDataContext, Glossaries, Tags) };
	}

	public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);
	public List<ISubViewModel> ViewModels { get; set; }
	private List<GlossaryModel> Glossaries { get; set; }

	private IInterpretBankDataContext InterpretBankDataContext { get; }
	private List<TagModel> Tags { get; set; }

	private void Save(object parameter)
	{
		InterpretBankDataContext.SubmitData();
	}
}