using System.Diagnostics.Contracts;

namespace Sdl.Community.CleanUpTasks.Contracts
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
