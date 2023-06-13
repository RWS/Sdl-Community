using System.Collections.ObjectModel;

namespace InterpretBank.SettingsService.Model
{
	public class GlossaryModel : ViewModel.ViewModel
	{
		private ObservableCollection<LanguageModel> _languages;
		private ObservableCollection<TagModel> _tags;
		public string GlossaryName { get; set; }
		public int Id { get; set; }

		public ObservableCollection<LanguageModel> Languages
		{
			get => _languages;
			set => SetField(ref _languages, value);
		}

		public ObservableCollection<TagModel> Tags
		{
			get => _tags;
			set => SetField(ref _tags, value);
		}
	}
}