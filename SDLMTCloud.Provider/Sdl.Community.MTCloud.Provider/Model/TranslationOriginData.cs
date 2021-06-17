using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationOriginData
	{
		public string Model { get; set; }
		public Dictionary<SegmentId, string> QualityEstimations { get; set; }
	}
}