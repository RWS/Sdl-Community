using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NLog;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier.Parsers.Number
{
	public class NumberNormalizer
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private INumberVerifierSettings _settings;
		private bool _isSource;

		public NumberList GetNormalizedNumbers(string text, INumberVerifierSettings settings, bool isSource)
		{
			_settings = settings;
			_isSource = isSource;

			NumberList numberList = null;
			try
			{
				var thousandSeparators = GetAllowedThousandSeparators();
				var decimalSeparators = GetAllowedDecimalSeparators();

				var numbers = NumberVerifier.Model.Number.Parse(text, thousandSeparators, decimalSeparators,
					isSource ? settings.SourceOmitLeadingZero : settings.TargetOmitLeadingZero);

				var initialParts = new List<string>();
				ToNumbers(numbers).ForEach(ip => initialParts.Add(ip.Normalize(NormalizationForm.FormKC)));

				var normalizedNumbers = new List<string>();
				ToNormalizedNumbers(numbers).ForEach(nn => normalizedNumbers.Add(nn.Normalize(NormalizationForm.FormKC)));

				numberList = new NumberList(initialParts, normalizedNumbers);
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}

			return numberList;
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

		private List<string> GetAllowedDecimalSeparators()
		{
			var sourceDecimalSeparators = _settings.GetSourceDecimalSeparators().ToList();
			var targetDecimalSeparators = _settings.GetTargetDecimalSeparators().ToList();


			if (_isSource) return sourceDecimalSeparators;

			var decimalSeparatorsList = new List<string>();
			ApplyTargetSettings(sourceDecimalSeparators, targetDecimalSeparators, decimalSeparatorsList);


			return decimalSeparatorsList;
		}

		private List<string> GetAllowedThousandSeparators()
		{
			var sourceThousandSeparators = _settings.GetSourceThousandSeparators().ToList();
			var targetThousandSeparators = _settings.GetTargetThousandSeparators().ToList();

			var thousandSeparatorsList = new List<string>();
			if (_settings.TargetNoSeparator || _settings.SourceNoSeparator) thousandSeparatorsList.Add(Constants.NoSeparator);

			if (_isSource) return thousandSeparatorsList.Concat(sourceThousandSeparators).ToList();

			ApplyTargetSettings(sourceThousandSeparators, targetThousandSeparators, thousandSeparatorsList);


			return thousandSeparatorsList;
		}
	}
}