using System.Diagnostics.Contracts;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks
{
	[ContractClass(typeof(IConversionFileViewPresenterContract))]
    public interface IConversionFileViewPresenter
    {
        void SaveFile(string lastUsedDirectory, bool isSaveAs);

        void CheckSaveButton();

        void Initialize();
    }
}