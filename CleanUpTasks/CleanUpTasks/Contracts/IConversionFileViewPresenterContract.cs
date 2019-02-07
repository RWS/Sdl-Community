using System.Diagnostics.Contracts;

namespace Sdl.Community.CleanUpTasks.Contracts
{
	[ContractClassFor(typeof(IConversionFileViewPresenter))]
    internal abstract class IConversionFileViewPresenterContract : IConversionFileViewPresenter
    {
        public void CheckSaveButton()
        {
        }

        public void Initialize()
        {
        }

        public void SaveFile(string lastUsedDirectory, bool isSaveAs)
        {
        }
    }
}