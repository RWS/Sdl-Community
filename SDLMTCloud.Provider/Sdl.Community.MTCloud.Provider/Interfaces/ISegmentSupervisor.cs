using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ISegmentSupervisor : ISupervisor<ImprovementFeedback>
	{
		event ShouldSendFeedbackEventHandler ShouldSendFeedback;

		ImprovementFeedback GetImprovement(SegmentId? segmentId = null);
	}
}