using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface ISupervisor<T>
	{
		ConcurrentDictionary<SegmentId, T> ActiveDocumentData { get; }
		Dictionary<Guid, ConcurrentDictionary<SegmentId, T>> Data { get; set; }

		void StartSupervising(ITranslationService translationService);
	}
}