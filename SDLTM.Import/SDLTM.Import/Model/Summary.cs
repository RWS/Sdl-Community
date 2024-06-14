using System.Collections.ObjectModel;

namespace SDLTM.Import.Model
{
    public class Summary:BaseModel
    {
	    private FileDetails _fileDetails;
	    private bool _importCompleted;
	    private bool _importStarted;
	    private ObservableCollection<TmDetails> _availableTms;

	    public FileDetails FileDetails
	    {
		    get => _fileDetails;
		    set
		    {
			    _fileDetails = value;
			    OnPropertyChanged(nameof(FileDetails));
		    }
	    }

	    public bool ImportCompeted
	    {
		    get => _importCompleted;
		    set
		    {
			    if (_importCompleted == value) return;
			    _importCompleted = value;
			    OnPropertyChanged(nameof(ImportCompeted));
		    }
	    }
	    public bool ImportStarted
	    {
		    get => _importStarted;
		    set
		    {
			    if (_importStarted == value) return;
			    _importStarted = value;
			    OnPropertyChanged(nameof(ImportStarted));
		    }
	    }

		public ObservableCollection<TmDetails> AvailableTms
	    {
		    get => _availableTms;
		    set
		    {
			    _availableTms = value;
			    OnPropertyChanged(nameof(AvailableTms));
		    }
	    }
    }
}
