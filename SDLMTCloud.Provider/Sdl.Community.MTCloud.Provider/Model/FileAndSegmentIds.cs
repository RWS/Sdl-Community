using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class FileAndSegmentIds
	{
		public string FilePath;
		public Dictionary<SegmentId, string> Segments;
	}
}