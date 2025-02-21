using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks.Dialogs
{
	[ContractClass(typeof(IFileDialogContract))]
    public interface IFileDialog
    {
        IEnumerable<string> GetFile(string lastUsedDirectory);

        string SaveFile(string lastUsedDirectory);
    }
}