using System;
using ProjectWizardExample.Model;
using ProjectWizardExample.Wizard.ViewModel;

namespace ProjectWizardExample.WizardPages.ViewModel
{
    public class Page02ViewModel : ProjectWizardViewModelBase
    {
	    private readonly Project _project;
		private bool _isValid;
	    private string _displayName;

        public Page02ViewModel(Project project, object view)
            : base(view)
        {
	        _project = project;
			_isValid = true;
	        _displayName = "Page 02";

			PropertyChanged += ViewModel_PropertyChanged;
		}

	    public override bool OnPageChange(int position, out string message)
	    {
		    message = string.Empty;

			if (position == PagePosition)
		    {
			    return false;
		    }

		    if (!IsValid && position > PagePosition)
		    {
				message = StringResources.UnableToNavigateToSelectedPage + Environment.NewLine + Environment.NewLine +
			              string.Format(StringResources.The_data_on__0__is_not_valid, _displayName);
			    return false;
		    }

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
                    return;

                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
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
