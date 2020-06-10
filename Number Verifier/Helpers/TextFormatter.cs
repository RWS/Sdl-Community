using System.Text.RegularExpressions;

namespace Sdl.Community.NumberVerifier.Helpers
{
	public class TextFormatter
	{
		private readonly NumberVerifierMain _numberVerifierMain;

		public TextFormatter(NumberVerifierMain numberVerifierMain)
		{
			_numberVerifierMain = numberVerifierMain;
		}

		// Format text so the 'No-break space'/'Thin space'/'Space'/'Narrow no break space' separators will be removed from the text, when the options are enabled
		// and Target or Source has NoSeparator option enabled:
		// E.g: source text: 2300 with option 'No separator' and target text: 2 300 with option 'No-break space'
		public string FormatTextSpace(string separators, string text)
		{
			if ((separators.Contains("u00A0") || separators.Contains("u2009") || separators.Contains("u0020") || separators.Contains("u202F"))
			    && (_numberVerifierMain.VerificationSettings.TargetNoSeparator || _numberVerifierMain.VerificationSettings.SourceNoSeparator))
			{
				text = Regex.Replace(text, @"\s+", string.Empty);
			}

			return text;
		}

		// Format text so the comma/period will be removed from the text when the TargetNoSeparator or SourceNoSeparator is enabled,
		// so the number can be validated entirely and it won't be split based on the , or .
		// E.g: source text: 1,300 with option 'Comma separator' and target text: 1300 with option 'No separator'
		public string FormatTextForNoSeparator(string text, bool isSource)
		{
			if (isSource && _numberVerifierMain.VerificationSettings.TargetNoSeparator || !isSource && _numberVerifierMain.VerificationSettings.SourceNoSeparator)
			{
				text = ReplaceSeparator(text, ",");
				text = ReplaceSeparator(text, ".");
			}
			return text;
		}

		private string ReplaceSeparator(string text, string separator)
		{
			if (!text.Contains(separator)) return text;
			text = Regex.Replace(text, $@"\{separator}", string.Empty);
			return text;
		}
	}
}