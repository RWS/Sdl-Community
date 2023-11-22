using System;
using System.Globalization;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace Multilingual.XML.FileType.Models
{
	public class LanguageMapping :ICloneable
	{
		public string LanguageId { get; set; }

		public string XPath { get; set; }

		public string CommentXPath { get; set; }
		
		[JsonIgnore]
		public string DisplayName { get; set; }

		[JsonIgnore]
		public BitmapImage Image { get; set; }

		public object Clone()
		{
			return new LanguageMapping
			{
				LanguageId = LanguageId,
				XPath = XPath,
				CommentXPath = CommentXPath,
				Image = Image?.Clone(),
				DisplayName = !string.IsNullOrEmpty(LanguageId) ? new CultureInfo(LanguageId).DisplayName : string.Empty
			};
		}
	}
}
