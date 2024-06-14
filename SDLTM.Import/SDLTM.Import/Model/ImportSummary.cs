namespace SDLTM.Import.Model
{
    public class ImportSummary:BaseModel
    {
	    private int _readTusCount;
	    private int _addedTusCount;
	    private int _errorCount;

	    public int ReadTusCount
	    {
		    get => _readTusCount;
		    set
		    {
			    _readTusCount = value;
			    OnPropertyChanged(nameof(ReadTusCount));
		    }
	    }

	    public int AddedTusCount
	    {
		    get => _addedTusCount;
		    set
		    {
			    _addedTusCount = value;
			    OnPropertyChanged(nameof(AddedTusCount));
		    }
	    }

	    public int ErrorCount
	    {
		    get => _errorCount;
		    set
		    {
			    _errorCount = value;
			    OnPropertyChanged(nameof(ErrorCount));
		    }
	    }
    }
}
