using System;
using System.Windows.Forms;
using log4net;
using log4net.Config;

namespace Sdl.Community.SDLXLIFFSliceOrChange
{
    static class Program
    {
        private static ILog logger = LogManager.GetLogger(typeof (Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            XmlConfigurator.Configure();
            Application.Run(new SDLXLIFFSliceOrChange());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Fatal("Unhandled exception.", e.ExceptionObject as Exception);
            MessageBox.Show(string.Format("A fatal exception has occurred: {0}", e.ExceptionObject));
        }
    }
}
