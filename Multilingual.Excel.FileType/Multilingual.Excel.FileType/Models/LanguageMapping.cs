using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace Multilingual.Excel.FileType.Models
{
	public class LanguageMapping : ICloneable
	{
		public string LanguageId { get; set; }

		public string ContentColumn { get; set; }

		public string CommentColumn { get; set; }

		public string ContextColumn { get; set; }

		public string CharacterLimitationColumn { get; set; }

		public string PixelLimitationColumn { get; set; }

		public string PixelFontFamilyColumn { get; set; }

		public string PixelFontSizeColumn { get; set; }

		public string PixelFontFamilyName { get; set; }

		public float PixelFontSize { get; set; }

		public bool IsDefault { get; set; }

		public bool ExcludeTranslations { get; set; }

		public bool FilterFillColorChecked { get; set; }

		public string FilterFillColor { get; set; }

		public string FilterScope { get; set; }


		[JsonIgnore]
		public string DisplayName { get; set; }

		[JsonIgnore]
		public BitmapImage Image { get; set; }

		public object Clone()
		{
			return new LanguageMapping
			{
				LanguageId = LanguageId,
				ContentColumn = ContentColumn,
				ContextColumn = ContextColumn,
				CommentColumn = CommentColumn,
				CharacterLimitationColumn = CharacterLimitationColumn,
				PixelLimitationColumn = PixelLimitationColumn,
				PixelFontFamilyColumn = PixelFontFamilyColumn,
				PixelFontSizeColumn = PixelFontSizeColumn,
				PixelFontFamilyName = PixelFontFamilyName,
				PixelFontSize = PixelFontSize,
				IsDefault = IsDefault,
				ExcludeTranslations = ExcludeTranslations,
				FilterFillColor = FilterFillColor,
				FilterFillColorChecked = FilterFillColorChecked,
				FilterScope = FilterScope,
				Image = Image?.Clone(),
				DisplayName = !string.IsNullOrEmpty(LanguageId) ? new CultureInfo(LanguageId).DisplayName : string.Empty
			};
		}
	}
}
