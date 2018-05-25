using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public class CheckBoxHeaderCellEventArgs : EventArgs
	{
		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
		}

		public CheckBoxHeaderCellEventArgs(bool _checked)
		{
			_isChecked = _checked;

		}
	}
}
