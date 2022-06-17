using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface IMetadataSupervisor : ISupervisor
	{
		string GetSegmentQe(SegmentId segmentId);
	}
}