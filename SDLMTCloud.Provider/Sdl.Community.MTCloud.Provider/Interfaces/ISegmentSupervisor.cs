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

		Dictionary<SegmentId, Feedback> ActiveDocumentImprovements { get; }

		Dictionary<Guid, Dictionary<SegmentId, Feedback>> Improvements { get; set; }

		void AddImprovement(SegmentId segmentId, string improvement);

		void CreateFeedbackEntry(SegmentId segmentId, string originalTarget, string targetOrigin,
			string source);

		Feedback GetImprovement(SegmentId? segmentId = null);

		void StartSupervising(ITranslationService translationService);
	}
}