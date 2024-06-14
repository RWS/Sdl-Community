using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;

namespace SDLTM.Import.Model
{
    public class Import:BaseModel
    {
	    private ObservableCollection<TmDetails> _tmsCollection;
	    private ObservableCollection<FileDetails> _fileDetails;
	    public CultureInfo SourceLanguage { get; set; }
	    public CultureInfo TargetLanguage { get; set; }
	    public Image SourceFlag { get; set; }
	    public Image TargetFlag { get; set; }

		public ObservableCollection<TmDetails> TmsCollection
	    {
		    get => _tmsCollection;
		    set
		    {
			    _tmsCollection = value;
			    OnPropertyChanged(nameof(TmsCollection));
		    }
	    }

	    public ObservableCollection<FileDetails> FilesCollection
	    {
		    get => _fileDetails;
		    set
		    {
			    _fileDetails = value;
			    OnPropertyChanged(nameof(FilesCollection));
		    }
	    }
    }
}
