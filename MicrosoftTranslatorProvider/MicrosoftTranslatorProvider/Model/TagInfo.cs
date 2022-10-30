using Sdl.LanguagePlatform.Core;

namespace MicrosoftTranslatorProvider.Model
{
	public class TagInfo
	{
		public string TagId { get; set; }

		public int Index { get; set; }

		public bool IsClosed { get; set; }

		public TagType TagType { get; set; }
	}
}