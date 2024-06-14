using System;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace SDLTM.Import.FileService
{
	public class LinguaTuBuilderSettings
	{
		public bool StripTags { get; set; }
		public bool ExcludeTagsInLockedContentText { get; set; }
		public bool AcceptTrackChanges { get; set; }
		public bool AlignTags { get; set; }
		public bool IncludeTrackChanges { get; set; }
		public bool IncludeComments { get; set; }
		public Func<IRevisionMarker, string> RevisionMarkerTagIdFunction { get; set; }
		public bool DoNotTrim { get; set; }
	}
}
