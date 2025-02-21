using System.Collections.Generic;
using LanguageWeaverProvider.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace LanguageWeaverProvider.BatchTask.Model
{
	public class TranslationOriginData
	{
		public string Model { get; set; }

		public Dictionary<SegmentId, RatedSegment> RatedSegments { get; set; }
	}
}