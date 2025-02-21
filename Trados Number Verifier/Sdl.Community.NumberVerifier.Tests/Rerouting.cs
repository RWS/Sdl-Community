using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Validator;

namespace Sdl.Community.NumberVerifier.Tests
{
	public static class Rerouting
	{
		private static NumberValidator _numberValidator;

		public static List<ErrorReporting> CheckSourceAndTarget(this NumberVerifierMain numberVerifierMain, string source, string target, [CallerMemberName] string caller = null)
		{
			if (caller?.Contains("Alphanumeric") ?? false)
			{
				return numberVerifierMain.CheckSegmentPair(source, target);
			}

			var numberVerifierSettings = numberVerifierMain.VerificationSettings;
			Setup();
			_numberValidator.Verify(source, target, numberVerifierSettings, out var sourceNumbersNormalized, out var targetNumbersNormalized, null, null);

			var sourceErrorList = CreateSimplifiedErrorReport(sourceNumbersNormalized, true);
			var targetErrorList = CreateSimplifiedErrorReport(targetNumbersNormalized, false);

			return sourceErrorList.Concat(targetErrorList).ToList();
		}

		private static List<ErrorReporting> CreateSimplifiedErrorReport(NumberTexts numberTexts, bool isSource)
		{
			var errorList = new List<ErrorReporting>();

			var sourceOrTarget = isSource ? "Source" : "Target";
			numberTexts.Texts.ForEach(t => t.ErrorsList.ForEach(e => errorList.Add(new ErrorReporting
			{
				Text = $"{sourceOrTarget}: {t.Text}",
				ErrorMessage = e.Message,
				ErrorType = e.ErrorLevel
			})));

			return errorList;
		}

		private static void Setup()
		{
			_numberValidator = new NumberValidator();
		}
	}
}