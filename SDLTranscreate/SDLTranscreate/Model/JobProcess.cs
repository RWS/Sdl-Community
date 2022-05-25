using System;
using System.Collections.Generic;

namespace Trados.Transcreate.Model
{
    public class JobProcess: BaseModel
    {
	    public enum ProcessStatus
	    {
		    Scheduled = 0,
		    Running = 1,
		    Completed = 2,
		    Failed = 3
	    }

	    private ProcessStatus _status;
	    private int _progress;
	    private string _name;
	    private string _description;

	    public JobProcess()
	    {
		    Errors = new List<Exception>();
		    Warnings = new List<Exception>();
		    Status = ProcessStatus.Scheduled;
	    }
	    
	    public string Name
		{
		    get => _name;
		    set
		    {
			    if (_name == value)
			    {
				    return;
			    }

			    _name = value;
			    OnPropertyChanged(nameof(Name));
		    }
	    }

	    public string Description
	    {
		    get => _description;
		    set
		    {
			    if (_description == value)
			    {
				    return;
			    }

			    _description = value;
			    OnPropertyChanged(nameof(Description));
		    }
	    }

		/// <summary>
		/// Scheduled, Running, Completed, Failed
		/// </summary>
		public ProcessStatus Status
	    {
		    get => _status;
		    set
		    {
			    if (_status == value)
			    {
				    return;
			    }

			    _status = value;
			    OnPropertyChanged(nameof(Status));
			    OnPropertyChanged(nameof(Message));
		    }
	    }

	    public int Progress
	    {
		    get => _progress;
		    set
		    {
			    if (_progress == value)
			    {
				    return;
			    }

			    _progress = value;
			    OnPropertyChanged(nameof(Progress));
		    }
	    }

		public string Message => $"Errors: {Errors.Count}, Warnings: {Warnings.Count}";

	    public bool HasErrors
	    {
		    get { return Errors.Count > 0; }
	    }

	    public bool HasWarnings
	    {
		    get { return Warnings.Count > 0; }
	    }

	    public List<Exception> Errors { get; set; }

	    public List<Exception> Warnings { get; set; }
	}
}
