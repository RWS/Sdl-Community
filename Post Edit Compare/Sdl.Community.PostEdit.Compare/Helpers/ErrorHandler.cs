using System;
using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Compare.Helpers
{
    public class ErrorHandler
    {
        public static void ShowError(Exception ex, IWin32Window owner)
        {
            MessageBox.Show(owner, $"{ex.Message}. ({ex.InnerException?.Message}) - {Environment.StackTrace}.",
                System.Windows.Forms.Application.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        
        public static void ShowError(string ex, IWin32Window owner)
        {
            MessageBox.Show(owner, $"{ex}) - {Environment.StackTrace}.",
                System.Windows.Forms.Application.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}