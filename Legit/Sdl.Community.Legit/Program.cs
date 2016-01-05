using System;
using System.Windows.Forms;
using Studio.AssemblyResolver;

namespace Sdl.Community.Legit
{
    static class Program
    {
        [STAThread]
        private static void Main()
        {
            AssemblyResolver.Resolve();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            
        }
    }
}
