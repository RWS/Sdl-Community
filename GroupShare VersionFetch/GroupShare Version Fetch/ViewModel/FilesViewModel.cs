using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
	public class FilesViewModel : ProjectWizardViewModelBase
	{
		private bool _isValid;

		public FilesViewModel(object view) : base(view)
		{
			_isValid = true;
		}

		public override string DisplayName => "Files";
		public override bool IsValid { get; set; }
	}
}
