using System.Collections.ObjectModel;

namespace SDLTM.Import.Model
{
	public class WizardModel : BaseModel
	{
		private ObservableCollection<TmDetails> _tmsCollection;
		private ObservableCollection<FileDetails> _filesCollection;
		private ObservableCollection<Summary> _summaryDetails;
		private ObservableCollection<Import> _importCollection;
		private Settings _importSettings;

		public ObservableCollection<TmDetails> TmsList
		{
			get => _tmsCollection;
			set
			{
				_tmsCollection = value;
				OnPropertyChanged(nameof(TmsList));
			}
		}

		public ObservableCollection<FileDetails> FilesList
		{
			get => _filesCollection;
			set
			{
				_filesCollection = value;
				OnPropertyChanged(nameof(FilesList));
			}
		}

		public ObservableCollection<Summary> SummaryDetails
		{
			get => _summaryDetails;
			set
			{
				_summaryDetails = value;
				OnPropertyChanged(nameof(SummaryDetails));
			}
		}

		public ObservableCollection<Import> ImportCollection
		{
			get => _importCollection;
			set
			{
				_importCollection = value;
				OnPropertyChanged(nameof(ImportCollection));
			}
		}

		public Settings ImportSettings
		{
			get => _importSettings;
			set
			{
				_importSettings = value;
				OnPropertyChanged(nameof(ImportSettings));
			}
		}
	}
}
