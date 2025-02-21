using System;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
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