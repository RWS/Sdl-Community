using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Events;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ISegmentSupervisor : ISupervisor
	{
		event ConfirmationLevelChangedEventHandler SegmentConfirmed;

		void AddImprovement(SegmentId segmentId, string improvement);

		void CreateFeedbackEntry(SegmentId segmentId, string originalTarget, string targetOrigin,
			string source);

		Feedback GetImprovement(SegmentId? segmentId = null);
	}
}