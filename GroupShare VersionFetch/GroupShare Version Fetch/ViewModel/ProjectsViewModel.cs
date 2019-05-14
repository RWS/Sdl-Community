using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class ProjectsViewModel: ProjectWizardViewModelBase
	{
		private bool _isValid;

		public ProjectsViewModel(object view) : base(view)
		{
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
