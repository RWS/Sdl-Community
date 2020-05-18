using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardPageOptionsViewModel : WizardPageViewModelBase
	{
		private string _displayName;
		private bool _isValid;

		public WizardPageOptionsViewModel(object view) : base(view)
		{
			_displayName = "Options";
			IsValid = true;

			PropertyChanged += WizardPageOptionsViewModel_PropertyChanged;
		}

		private void WizardPageOptionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(CurrentPageChanged))
			{
				if (IsCurrentPage)
				{
					
				}
			}
		}

		public override string DisplayName => _displayName;

		public override bool IsValid
		{
			get => _isValid;
			set => _isValid = value;
		}
	}
}
