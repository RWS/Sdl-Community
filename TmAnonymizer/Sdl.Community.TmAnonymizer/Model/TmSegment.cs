using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class TmSegment
	{						
		public List<SegmentElement> Elements { get; set; }		

		public string Language { get; set; }

		public string ToPlain()
		{
			var builder = new StringBuilder();
			if (Elements != null)
			{
				foreach (var element in Elements)
				{
					if (element != null)
					{
						if (element is Text text)
						{
							builder.Append(text.Value);
						}
						else if (element is Tag)
						{
							var tag = (Tag)element;
							if ((tag.Type == TagType.TextPlaceholder) || (tag.Type == TagType.LockedContent))
							{
								builder.Append(tag.TextEquivalent);
							}
						}
					}
				}
			}
			return builder.ToString();
		}
	}
}
