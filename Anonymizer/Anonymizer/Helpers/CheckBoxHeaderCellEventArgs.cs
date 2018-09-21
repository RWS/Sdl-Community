using System;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public class CheckBoxHeaderCellEventArgs : EventArgs
	{
		private readonly bool _isChecked;

		public CheckBoxHeaderCellEventArgs(bool _checked)
		{
			_isChecked = _checked;
		}

		public bool IsChecked
		{
			get { return _isChecked; }
		}
	}
}