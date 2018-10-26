using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	[Serializable]
	public class TmSegment : ICloneable
	{
		public List<SegmentElement> Elements { get; set; }

		public string Language { get; set; }

		public string ToPlain(bool includeTags = false)
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
								if (string.IsNullOrEmpty(tag.TextEquivalent) && includeTags)
								{
									builder.Append("<" + tag.TagID + " text-equiv=\"\" />");
								}
								else
								{
									builder.Append(tag.TextEquivalent);
								}
							}
						}
					}
				}
			}
			return builder.ToString();
		}

		public object Clone()
		{
			var elements = new List<SegmentElement>();
			foreach (var element in Elements)
			{				
				switch (element)
				{
					case Text txt:
						{
							elements.Add(new Text(txt.Value));
							break;
						}

					case Tag tag:
						{
							elements.Add(new Tag(tag.Type, tag.TagID, tag.Anchor, tag.AlignmentAnchor, tag.TextEquivalent, tag.CanHide));
							break;
						}
				}
			}

			return new TmSegment
			{
				Elements = elements,
				Language = Language
			};
		}
	}
}
