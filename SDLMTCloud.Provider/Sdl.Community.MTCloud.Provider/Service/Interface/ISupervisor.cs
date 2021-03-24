using System;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface ISupervisor
	{
		Dictionary<SegmentId, TargetSegmentData> ActiveDocumentData { get; }
		Dictionary<Guid, Dictionary<SegmentId, TargetSegmentData>> Data { get; set; }

		void StartSupervising(ITranslationService translationService);
	}
}