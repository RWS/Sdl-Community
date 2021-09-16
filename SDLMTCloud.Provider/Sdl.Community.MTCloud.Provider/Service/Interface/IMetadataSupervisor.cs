using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface IMetadataSupervisor : ISupervisor<TranslationOriginDatum>
	{
		string GetSegmentQe(SegmentId segmentId);
	}
}