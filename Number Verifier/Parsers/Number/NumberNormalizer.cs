using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns.Specialized;

namespace Sdl.Community.NumberVerifier.Parsers.Number
{
	public class NumberNormalizer
	{
		private INumberVerifierSettings _settings;

		public void GetNormalizedNumbers(string sourceText, string targetText, INumberVerifierSettings settings, out NumberList sourceNumberList, out NumberList targetNumberList)
		{
			_settings = settings;
			(sourceNumberList, targetNumberList) = GetNormalizedNumbers(sourceText, targetText, settings);
		}

		private void ApplyTargetSettings(List<string> sourceSeparators, List<string> targetSeparators, List<string> separatorsList)
		{
			if (_settings.PreventLocalizations)
			{
				separatorsList.AddRange(sourceSeparators);
			}
			if (_settings.AllowLocalizations)
			{
				separatorsList.AddRange(sourceSeparators);
				separatorsList.AddRange(targetSeparators);
			}
			if (_settings.RequireLocalizations)
			{
				separatorsList.AddRange(targetSeparators);
			}
		}

		private List<string> GetAllowedDecimalSeparators(bool isSource)
		{
			var sourceDecimalSeparators = _settings.GetSourceDecimalSeparators().ToList();
			var targetDecimalSeparators = _settings.GetTargetDecimalSeparators().ToList();

			var decimalSeparatorsList = new List<string>();
			if (isSource) decimalSeparatorsList = sourceDecimalSeparators;

			else ApplyTargetSettings(sourceDecimalSeparators, targetDecimalSeparators, decimalSeparatorsList);

			return decimalSeparatorsList.Select(Regex.Unescape).ToList();
		}

		private List<string> GetAllowedThousandSeparators(bool isSource)
		{
			var sourceThousandSeparators = _settings.GetSourceThousandSeparators().ToList();
			var targetThousandSeparators = _settings.GetTargetThousandSeparators().ToList();

			var thousandSeparatorsList = new List<string>();
			if (_settings.TargetNoSeparator || _settings.SourceNoSeparator) thousandSeparatorsList.Add(Constants.NoSeparator);

			if (isSource) thousandSeparatorsList.AddRange(sourceThousandSeparators);

			else ApplyTargetSettings(sourceThousandSeparators, targetThousandSeparators, thousandSeparatorsList);

			return thousandSeparatorsList.Select(Regex.Unescape).ToList();
		}

		private (NumberList, NumberList) GetNormalizedNumbers(string sourceText, string targetText, INumberVerifierSettings settings)
		{
			var sourceThousandSeparators = GetAllowedThousandSeparators(true);
			var sourceDecimalSeparators = GetAllowedDecimalSeparators(true).Select(Regex.Unescape).ToList();

			var targetThousandSeparators = GetAllowedThousandSeparators(false);
			var targetDecimalSeparators = GetAllowedDecimalSeparators(false).Select(Regex.Unescape).ToList();

			var sourceRealNumberPattern = new RealNumber(sourceThousandSeparators, sourceDecimalSeparators, _settings.SourceOmitLeadingZero);
			var targetRealNumberPattern = new RealNumber(targetThousandSeparators, targetDecimalSeparators, _settings.TargetOmitLeadingZero);
			sourceRealNumberPattern.MatchAll(sourceText);
			targetRealNumberPattern.MatchAll(targetText);

			var sourceNumbers = sourceRealNumberPattern.ToNumberList()?.Select(n=>n.Normalize(NormalizationForm.FormKC)).ToList();
			var sourceNormalizedNumberList = sourceRealNumberPattern.ToNormalizedNumberList()?.Select(n => n.Normalize(NormalizationForm.FormKC)).ToList();

			var targetNumbers = targetRealNumberPattern.ToNumberList()?.Select(n => n.Normalize(NormalizationForm.FormKC)).ToList();
			var targetNormalizedNumberList = targetRealNumberPattern.ToNormalizedNumberList()?.Select(n => n.Normalize(NormalizationForm.FormKC)).ToList();


			return
				(
					new NumberList(sourceNumbers, sourceNormalizedNumberList),
						new NumberList(targetNumbers, targetNormalizedNumberList)
					);
		}
	}
}