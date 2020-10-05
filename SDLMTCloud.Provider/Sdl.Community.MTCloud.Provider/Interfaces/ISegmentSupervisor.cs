using System;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Service.Events;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ISegmentSupervisor
	{
		event ConfirmationLevelChangedEventHandler SegmentConfirmed;

		Dictionary<SegmentId, ImprovedTarget> ActiveDocumentImprovements { get; }
		Dictionary<Guid, Dictionary<SegmentId, ImprovedTarget>> Improvements { get; set; }

		void StartSupervising();

		void StopSupervising();
		ImprovedTarget GetImprovement(SegmentId? segmentId = null);
	}
}