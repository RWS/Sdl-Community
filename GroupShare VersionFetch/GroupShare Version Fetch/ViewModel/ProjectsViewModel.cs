using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.GSVersionFetch.Model;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class ProjectsViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;
		private WizardModel _wizardModel;
		public ProjectsViewModel(WizardModel wizardModel, object view) : base(view)
		{
			_wizardModel = wizardModel;
		}

		public override string DisplayName => "GroupShare Projects";
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
	}
}
