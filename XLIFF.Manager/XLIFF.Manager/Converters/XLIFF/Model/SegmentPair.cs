using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XLIFF.Manager.Converters.XLIFF.Model
{
	public class SegmentPair
	{
		public string Id { get; set; }

		public Source Source { get; set; }

		public Target Target { get; set; }

		public bool IsLocked { get; set; }

		public ConfirmationLevel ConfirmationLevel { get; set; }

		public ITranslationOrigin TranslationOrigin { get; set; }

		
	}
}
