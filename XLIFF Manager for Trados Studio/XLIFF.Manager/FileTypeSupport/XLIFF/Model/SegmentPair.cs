using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model
{
	public class SegmentPair
	{		
		public SegmentPair(SegmentBuilder segmentBuilder)
		{
			ConfirmationLevel = ConfirmationLevel.Unspecified;
			TranslationOrigin = segmentBuilder.CreateTranslationOrigin();
			Source = new Source();
			Target = new Target();
		}

		public string Id { get; set; }

		public Source Source { get; set; }

		public Target Target { get; set; }

		public bool IsLocked { get; set; }

		public ConfirmationLevel ConfirmationLevel { get; set; }

		public ITranslationOrigin TranslationOrigin { get; set; }		
	}
}
