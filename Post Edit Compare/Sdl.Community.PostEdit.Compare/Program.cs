using System;
using PostEdit.Compare.Model;
using System.Windows.Forms;

namespace PostEdit.Compare
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            IModel mModel = new Model.Model();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain(mModel));
        }
    }
}
