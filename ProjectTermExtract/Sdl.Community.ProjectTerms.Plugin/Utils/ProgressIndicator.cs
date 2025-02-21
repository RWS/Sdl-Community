using Sdl.Community.ProjectTerms.Controls.Interfaces;

namespace Sdl.Community.ProjectTerms.Plugin.Utils
{
    class ProgressIndicator : IProgressIndicator
    {
        public int Maximum { get; set; }

        public void Increment(int value) { }
    }
}
