using System.Diagnostics.Contracts;
using Sdl.Community.CleanUpTasks.Contracts;

namespace Sdl.Community.CleanUpTasks
{
	[ContractClass(typeof(ISegmentLockerPresenterContract))]
    public interface ISegmentLockerPresenter
    {
        void Initialize();
        void SaveSettings();
    }
}