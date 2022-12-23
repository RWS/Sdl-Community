using Sdl.LanguagePlatform.Core;

namespace GoogleCloudTranslationProvider.Models
{
	public class TagInfo
	{
		public int Index { get; set; }

		public bool IsClosed { get; set; }

		public string TagId { get; set; }

		public TagType TagType { get; set; }
	}
}