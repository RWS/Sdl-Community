using ProjectWizardExample.Model;
using ProjectWizardExample.Wizard.ViewModel;

namespace ProjectWizardExample.WizardPages.ViewModel
{
    public class Page05ViewModel : ProjectWizardViewModelBase
    {
	    private readonly Project _project;
		private bool _isValid;
	    private string _displayName;

		public Page05ViewModel(Project project, object view)
            : base(view)
        {
	        _project = project;
			_isValid = true;
	        _displayName = "Page 05";

			IsComplete = false;
			
			PropertyChanged += ViewModel_PropertyChanged;
		}

	    public override bool OnChangePage(out string message)
	    {
		    message = string.Empty;		   
		    return true;
	    }

		public override string DisplayName
	    {
		    get => _displayName;
		    set
		    {
			    if (_displayName == value)
			    {
				    return;
			    }

			    _displayName = value;
			    OnPropertyChanged(nameof(DisplayName));
		    }
	    }

		public override bool IsValid
        {
            get => _isValid;
            set
            {
	            if (_isValid == value)
	            {
		            return;
	            }

				_isValid = value;	            
				OnPropertyChanged(nameof(IsValid));

				if (!_isValid)
				{
					IsComplete = false;
					OnPropertyChanged(nameof(IsComplete));
				}
			}
        }		

	    private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	    {
		    if (e.PropertyName == nameof(CurrentPageChanged))
		    {
			    if (IsCurrentPage)
			    {
				    // do something                     
			    }
		    }
	    }
	}
}
