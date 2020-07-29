using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public class TagInfo
    {
        public int Index { get; set; }
        public bool IsClosed { get; set; }
        public string TagId { get; set; }
        public TagType TagType { get; set; }
    }
}