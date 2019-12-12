using System.Collections.Generic;
using SDLCommunityCleanUpTasks.Dialogs;

namespace SDLCommunityCleanUpTasks.Contracts
{
    internal abstract class IFileDialogContract : IFileDialog
    {
        public IEnumerable<string> GetFile(string lastUsedDirectory)
        {
            return default(IEnumerable<string>);
        }

        public string SaveFile(string lastUsedDirectory)
        {
            return default(string);
        }
    }
}