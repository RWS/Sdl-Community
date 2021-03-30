using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class PackageDetailsViewModel:WizardViewModelBase
	{
		public PackageDetailsViewModel(object view) : base(view)
		{
		}

		public override string Tooltip { get; set; }
		public override bool OnChangePage(int position, out string message)
		{
			message = string.Empty;
			//throw new NotImplementedException();
			return true;
		}

		public override string DisplayName { get; set; }
		public override bool IsValid { get; set; }
	}
}
