using System.Linq;

namespace GoogleTranslatorProvider.Extensions
{
	public static class StringExtensions
	{
		public static string EncodeSpecialChars(this string text)
		{
			text = text.Replace("#", "%23");
			text = text.Replace("&", "%26");
			text = text.Replace(";", "%3b");
			return text;
		}

		public static string RemoveZeroWidthSpaces(this string text)
		{
			var charArr = text.ToCharArray()
							  .Where(val => val != (char)8203)
							  .ToArray();
			return new string(charArr);
		}
	}
}