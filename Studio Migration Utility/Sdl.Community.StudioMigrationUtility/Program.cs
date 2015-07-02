using System;
using System.Windows.Forms;
using Sdl.Community.StudioMigrationUtility.Services;

namespace Sdl.Community.StudioMigrationUtility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MigrateUtility(new StudioVersionService()));
        }
    }
}
