using System.ComponentModel;

namespace PostEdit.Compare
{


    public class ProgressWindow
    {
        public static BackgroundWorker ProgressDialogWorker;
        public static ProgressDialog ProgressDialog;
    }

    public class ProgressWindowHandlers
    {
        public static void progressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressWindow.ProgressDialog.ProgressObject = e.UserState;
        }

        public static void progressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressWindow.ProgressDialog.Close();
        }

        
    }



    public class WaitingWindow
    {
        public static BackgroundWorker WaitingDialogWorker;
        public static WaitingDialog WaitingDialog;
    }

    public class WaitingWindowHandlers
    {
        public static void waitingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WaitingWindow.WaitingDialog.ProgressObject = e.UserState;
        }

        public static void waitingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WaitingWindow.WaitingDialog.Close();
        }
    }
}
