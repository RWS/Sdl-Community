using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Events;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ISegmentSupervisor
	{
		event ConfirmationLevelChangedEventHandler ConfirmationLevelChanged;

		Dictionary<SegmentId, ImprovedTarget> Improvements { get; set; }

		void StartSupervising();

		void StopSupervising();
	}
}