using System.Diagnostics.Contracts;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks
{
	[ContractClass(typeof(ISegmentLockerPresenterContract))]
    public interface ISegmentLockerPresenter
    {
        void Initialize();

        void SaveSettings();
    }
}