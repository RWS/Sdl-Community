using System.Collections.Generic;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace LanguageWeaverProvider.BatchTask.Model
{
	public class MetadataTransferObject
	{
		public string FilePath { get; set; }

		
		public string TargetLanguage { get; set; }
		
		public List<SegmentId> SegmentIds { get; set; }
		
		public TranslationOriginData TranslationOriginData { get; set; }
	}
}