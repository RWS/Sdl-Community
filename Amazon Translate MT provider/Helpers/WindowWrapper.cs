using System;
using System.Windows.Forms;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Helpers
{
    /// <summary>
    /// this class allows getting the Studio application window as an IWin32Window to be used in modal dialogs
    /// </summary>
    class WindowWrapper : IWin32Window
    {
        public WindowWrapper(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; }
    }
}