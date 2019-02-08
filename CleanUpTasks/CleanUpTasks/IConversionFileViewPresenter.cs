using System.Diagnostics.Contracts;
using Sdl.Community.CleanUpTasks.Contracts;

namespace Sdl.Community.CleanUpTasks
{
	[ContractClass(typeof(IConversionFileViewPresenterContract))]
    public interface IConversionFileViewPresenter
    {
        void SaveFile(string lastUsedDirectory, bool isSaveAs);
        void CheckSaveButton();
        void Initialize();
    }
}