using System;

namespace Multilingual.Excel.FileType.Models
{
	public abstract class Element: ICloneable
	{
		public enum TagType
		{
			TagOpen,
			TagClose,
		}

		public object Clone()
		{
			if (this is ElementComment comment)
			{
				return new ElementComment
				{
					Id = comment.Id,
					Type = comment.Type
				};
			}

			if (this is ElementLocked locked)
			{
				return new ElementLocked
				{
					Type = locked.Type
				};
			}

			if (this is ElementPlaceholder placeholder)
			{
				return new ElementPlaceholder
				{
					DisplayText = placeholder.DisplayText,
					TagContent = placeholder.TagContent,
					TagId = placeholder.TagId,
					TextEquivalent = placeholder.TextEquivalent
				};
			}

			if (this is ElementSegment segment)
			{
				return new ElementSegment
				{
					Id = segment.Id,
					Type = segment.Type
				};
			}

			if (this is ElementTagPair tagPair)
			{
				return new ElementTagPair
				{
					DisplayText = tagPair.DisplayText,
					TagContent = tagPair.TagContent,
					TagId = tagPair.TagId,
					Type = tagPair.Type
				};
			}

			if (this is ElementText text)
			{
				return new ElementText
				{
					Text = text.Text
				};
			}

			return MemberwiseClone();
		}
	}
}
