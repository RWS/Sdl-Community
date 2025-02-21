using System;
using System.Windows.Forms;

namespace GoogleCloudTranslationProvider.Models
{
	class WindowWrapper : IWin32Window
	{
		public WindowWrapper(IntPtr handle)
		{
			Handle = handle;
		}

		public IntPtr Handle { get; }
	}
}