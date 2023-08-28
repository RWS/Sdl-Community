using Sdl.Community.DeepLMTProvider.Interface;
using System.Diagnostics;

namespace Sdl.Community.DeepLMTProvider.Helpers
{
    public class ProcessStarter : IProcessStarter
    {
        public void StartInFileExplorer(string path)
        {
            Process.Start(path);
        }
    }
}