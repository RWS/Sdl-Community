using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Parsers.Number.GenericParser.Patterns.Specialized;
using Sdl.Community.NumberVerifier.Parsers.Number.RealNumberParser;

namespace Sdl.Community.NumberVerifier.Parsers.Number
{
	public class NumberNormalizer
	{
		private INumberVerifierSettings _settings;

		public void GetNormalizedNumbers(string sourceText, string targetText, INumberVerifierSettings settings, out NumberList sourceNumberList, out NumberList targetNumberList)
		{
			_settings = settings;
			(sourceNumberList, targetNumberList) = GetNormalizedNumbers2(sourceText, targetText, settings);
			
			//(sourceNumberList, targetNumberList) = GetNormalizedNumbers(sourceText, targetText, settings);
		}

		public List<string> ToNormalizedNumbers(List<NumberVerifier.Model.Number> numbers)
		{
			return numbers.Select(n => n.ToNumber(true)).ToList();
		}

		public List<string> ToNumbers(List<NumberVerifier.Model.Number> numbers)
		{
			return numbers.Select(n => n.ToNumber()).ToList();
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

			ApplyTargetSettings(sourceThousandSeparators, targetThousandSeparators, thousandSeparatorsList);

			return thousandSeparatorsList.Select(Regex.Unescape).ToList();
		}

		private (NumberList, NumberList) GetNormalizedNumbers2(string sourceText, string targetText, INumberVerifierSettings settings)
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

		private (NumberList, NumberList) GetNormalizedNumbers(string sourceText, string targetText, INumberVerifierSettings settings)
		{
			var sourceThousandSeparators = GetAllowedThousandSeparators(true);
			var sourceDecimalSeparators = GetAllowedDecimalSeparators(true);

			var targetThousandSeparators = GetAllowedThousandSeparators(false);
			var targetDecimalSeparators = GetAllowedDecimalSeparators(false);

			var sourceNumbers = new List<NumberVerifier.Model.Number>();
			if (sourceText != null)
			{
				sourceNumbers = NumberVerifier.Model.Number.Parse(sourceText, sourceThousandSeparators, sourceDecimalSeparators,
					settings.SourceOmitLeadingZero);
			}

			var targetNumbers = new List<NumberVerifier.Model.Number>();
			if (targetText != null)
			{
				targetNumbers = NumberVerifier.Model.Number.Parse(targetText, targetThousandSeparators, targetDecimalSeparators,
					settings.TargetOmitLeadingZero, sourceNumbers);
			}

			var initialSourceNumbers = new List<string>();
			ToNumbers(sourceNumbers).ForEach(ip => initialSourceNumbers.Add(ip.Normalize(NormalizationForm.FormKC)));

			var normalizedSourceNumbers = new List<string>();
			ToNormalizedNumbers(sourceNumbers).ForEach(nn => normalizedSourceNumbers.Add(nn.Normalize(NormalizationForm.FormKC)));


			var initialTargetNumbers = new List<string>();
			ToNumbers(targetNumbers).ForEach(ip => initialTargetNumbers.Add(ip.Normalize(NormalizationForm.FormKC)));

			var normalizedTargetNumbers = new List<string>();
			ToNormalizedNumbers(targetNumbers).ForEach(nn => normalizedTargetNumbers.Add(nn.Normalize(NormalizationForm.FormKC)));

			return
				(
					sourceNumbers.Count > 0 ? new NumberList(initialSourceNumbers, normalizedSourceNumbers) : null,
						targetNumbers.Count > 0 ? new NumberList(initialTargetNumbers, normalizedTargetNumbers) : null);

		}
	}
}