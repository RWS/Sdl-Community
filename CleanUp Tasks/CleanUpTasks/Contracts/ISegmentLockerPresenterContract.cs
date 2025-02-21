using System.Diagnostics.Contracts;

namespace SDLCommunityCleanUpTasks.Contracts
{
	[ContractClassFor(typeof(ISegmentLockerPresenter))]
    internal abstract class ISegmentLockerPresenterContract : ISegmentLockerPresenter
    {
        public void Initialize()
        {
        }

        public void SaveSettings()
        {
        }
    }
}