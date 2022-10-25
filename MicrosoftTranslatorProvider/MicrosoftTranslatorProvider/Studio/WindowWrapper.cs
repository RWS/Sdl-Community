using System;

namespace MicrosoftTranslatorProvider.Studio
{
	public class WindowWrapper : System.Windows.Forms.IWin32Window
	{
		public WindowWrapper(IntPtr handle)
		{
			Handle = handle;
		}

		public IntPtr Handle { get; }
	}
}