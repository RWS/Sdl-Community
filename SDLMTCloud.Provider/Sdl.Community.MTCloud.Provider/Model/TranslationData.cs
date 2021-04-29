using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class TranslationData
	{
		public string FilePath { get; set; }
		public Dictionary<SegmentId, string> Segments { get; set; }
		public List<string> SourceSegments { get; set; }
		public string TargetLanguage { get; set; }
		public List<string> TargetSegments { get; set; }

		public TranslationOriginInformation TranslationOriginInformation { get; set; }
	}
}