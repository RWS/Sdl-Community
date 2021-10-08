using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Parsers.Number
{
	public class NumberNormalizer
	{
		private INumberVerifierSettings _settings;

		public void GetNormalizedNumbers(string sourceText, string targetText, INumberVerifierSettings settings, out NumberList sourceNumberList, out NumberList targetNumberList)
		{
			_settings = settings;
			sourceNumberList = GetNormalizedNumbers(sourceText, settings, true);
			targetNumberList = GetNormalizedNumbers(targetText, settings, false);
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

			if (isSource) return sourceDecimalSeparators;

			var decimalSeparatorsList = new List<string>();
			ApplyTargetSettings(sourceDecimalSeparators, targetDecimalSeparators, decimalSeparatorsList);

			return decimalSeparatorsList;
		}

		private List<string> GetAllowedThousandSeparators(bool isSource)
		{
			var sourceThousandSeparators = _settings.GetSourceThousandSeparators().ToList();
			var targetThousandSeparators = _settings.GetTargetThousandSeparators().ToList();

			var thousandSeparatorsList = new List<string>();
			if (_settings.TargetNoSeparator || _settings.SourceNoSeparator) thousandSeparatorsList.Add(Constants.NoSeparator);

			if (isSource) return thousandSeparatorsList.Concat(sourceThousandSeparators).ToList();

			ApplyTargetSettings(sourceThousandSeparators, targetThousandSeparators, thousandSeparatorsList);

			return thousandSeparatorsList;
		}

		private NumberList GetNormalizedNumbers(string sourceText, INumberVerifierSettings settings, bool isSource)
		{
			if (sourceText is null) return null;
			var thousandSeparators = GetAllowedThousandSeparators(isSource);
			var decimalSeparators = GetAllowedDecimalSeparators(isSource);

			var numbers = NumberVerifier.Model.Number.Parse(sourceText, thousandSeparators, decimalSeparators,
				isSource ? settings.SourceOmitLeadingZero : settings.TargetOmitLeadingZero);

			var initialParts = new List<string>();
			ToNumbers(numbers).ForEach(ip => initialParts.Add(ip.Normalize(NormalizationForm.FormKC)));

			var normalizedNumbers = new List<string>();
			ToNormalizedNumbers(numbers).ForEach(nn => normalizedNumbers.Add(nn.Normalize(NormalizationForm.FormKC)));

			return new NumberList(initialParts, normalizedNumbers);
		}
	}
}