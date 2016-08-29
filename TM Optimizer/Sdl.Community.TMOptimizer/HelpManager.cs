using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sdl.Community.TMOptimizer
{
     public static class HelpManager
    {
        public static void ShowHelp()
        {
            string dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(Path.Combine(dir, @"TMOptimizerHelp\help.htm"));
        }
    }
}
