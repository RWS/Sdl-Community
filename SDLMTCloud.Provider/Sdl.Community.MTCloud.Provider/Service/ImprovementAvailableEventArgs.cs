using System;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public partial class SegmentSupervisor
	{
		public class ImprovementAvailableEventArgs : EventArgs
		{
			public string ConfirmedSegment { get; set; }
		}
	}
}