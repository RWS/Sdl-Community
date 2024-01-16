using System.Collections.ObjectModel;

namespace InterpretBank.SettingsService.Model
{
	public class GlossaryModel : ViewModelBase.ViewModel
    {
        private ObservableCollection<LanguageModel> _languages = new();
		private ObservableCollection<TagModel> _tags = new();
		public string GlossaryName { get; set; }
		public string SubGlossaryName { get; set; }
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

        public override string ToString() => $"{GlossaryName}{SubGlossaryName}";
    }
}