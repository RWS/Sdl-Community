using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class TmTranslationUnit
	{
		public PersistentObjectToken ResourceId { get; set; }
		public TmSegment SourceSegment { get; set; }
		public TmSegment TargetSegment { get; set; }
		public SystemFields SystemFields { get; set; }
		public FieldValues FieldValues { get; set; }
	}
}
