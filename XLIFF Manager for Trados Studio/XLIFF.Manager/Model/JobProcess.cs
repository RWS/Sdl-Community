using System;
using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.Model
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

	    public JobProcess()
	    {
		    Errors = new List<Exception>();
		    Warnings = new List<Exception>();
		    Status = ProcessStatus.Scheduled;
	    }

	    public string Name { get; set; }

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

	    public string Message
	    {
		    get { return $"Errors: {Errors.Count}, Warnings: {Warnings.Count}"; }
	    }

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
