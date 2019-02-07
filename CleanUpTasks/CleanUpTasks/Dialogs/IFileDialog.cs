using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Sdl.Community.CleanUpTasks.Contracts;

namespace Sdl.Community.CleanUpTasks.Dialogs
{
	[ContractClass(typeof(IFileDialogContract))]
    public interface IFileDialog
    {
        IEnumerable<string> GetFile(string lastUsedDirectory);

        string SaveFile(string lastUsedDirectory);
    }
}