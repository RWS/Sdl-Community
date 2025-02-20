using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Compare.Helpers
{
    public class ErrorHandler
    {
        public static void ShowError(Exception ex, IWin32Window owner = null, [CallerMemberName] string callingMethod = "")
        {
            MessageBox.Show(owner, $"{ex.Message}. ({ex.InnerException?.Message}) - {ex.StackTrace}.",
                callingMethod,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void ShowError(string message, IWin32Window owner = null, [CallerMemberName] string callingMethod = "") => MessageBox.Show(owner,
            message,
            callingMethod,
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}