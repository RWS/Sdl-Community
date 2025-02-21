using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Models
{
	public class TagPair
	{
		public IStartTagProperties StartTag { get; }
		public IEndTagProperties EndTag { get; }

		public TagPair(IStartTagProperties startTag, IEndTagProperties endTag)
		{
			StartTag = startTag;
			EndTag = endTag;
		}
	}
}
