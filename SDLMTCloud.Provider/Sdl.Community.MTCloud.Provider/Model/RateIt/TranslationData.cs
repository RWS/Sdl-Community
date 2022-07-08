using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Model.RateIt
{
	public class TranslationData
	{
		public string FilePath { get; set; }
		public string TargetLanguage { get; set; }
		public Dictionary<SegmentId, string> Segments { get; set; }
		public Dictionary<SegmentId, string> TargetSegments { get; set; }
		public TranslationOriginData TranslationOriginData{ get; set; }
	}
}