using System;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface ISupervisor<T>
	{
		Dictionary<SegmentId, T> ActiveDocumentData { get; }
		Dictionary<Guid, Dictionary<SegmentId, T>> Data { get; set; }

		void StartSupervising(ITranslationService translationService);
	}
}