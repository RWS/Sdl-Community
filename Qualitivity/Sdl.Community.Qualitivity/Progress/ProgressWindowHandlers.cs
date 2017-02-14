using System.ComponentModel;

namespace Sdl.Community.Qualitivity.Progress
{
    public class ProgressWindowHandlers
    {
        public static void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            ProgressWindow.ProgressDialog.ProgressObject = e.UserState;
        }

        public static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressWindow.ProgressDialog.Close();
        }
    }
}
