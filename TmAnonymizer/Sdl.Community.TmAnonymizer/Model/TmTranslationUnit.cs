using System;
using System.Collections.Generic;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	[Serializable]
	public class TmTranslationUnit
	{
		public PersistentObjectToken ResourceId { get; set; }

		public int TmId { get; set; }

		public TmSegment SourceSegment { get; set; }

		public TmSegment TargetSegment { get; set; }

		public SystemFields SystemFields { get; set; }

		public List<FieldDefinitions.FieldValue> FieldValues { get; set; }
	}
}
