using System.Windows.Forms;
using Sdl.Community.Productivity.UI;

namespace Sdl.Community.Productivity.Test
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
           Application.Run(new ProductivityForm());
        }
    }
}
