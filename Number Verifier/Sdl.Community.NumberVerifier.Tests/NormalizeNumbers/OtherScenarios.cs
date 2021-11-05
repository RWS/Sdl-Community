using Sdl.Community.NumberVerifier.Validator;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class OtherScenarios
	{
		private readonly NumberValidator _numberValidator;
		private readonly SettingsBuilder _settingsBuilder;

		public OtherScenarios()
		{
			_numberValidator = new NumberValidator();
			_settingsBuilder = new SettingsBuilder();
		}

		[Theory]
		[InlineData("11,200.300", "11,200.300")]
		[InlineData("1,234.89", "١,٢٣٤.٨٩")]
		public void ErrorScenarios_WhenDecimalSeparatorsDifferent_LocalizationAllowed(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.WithSourceThousandSeparators(comma: false, period: true)
					.WithTargetThousandSeparators(comma: true, period: false)
					.WithSourceDecimalSeparators(comma: true, period: false)
					.WithTargetDecimalSeparators(comma: false, period: true)
					.Build();

			settings.Setup(s => s.HindiNumberVerification).Returns(true);

			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings: settings.Object);
			numberVerifierMain.Initialize(documentInfo: null);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(sourceText: source, targetText: target);

			Assert.Collection(collection: errorMessage,
				m => Assert.Equal(expected: PluginResources.ThousandSeparatorAfterDecimal, actual: m.ErrorMessage),
				m => Assert.Equal(expected: PluginResources.Error_SameSequencesButDifferentValues, actual: m.ErrorMessage));
		}

		[Theory]
		[InlineData("11t200d300", "11t200d300")]
		public void NoAlphanumericErrors_WhenCustomSeparatorsAreUsed(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.WithSourceThousandSeparators(false, true, "t")
					.WithTargetThousandSeparators(true, false, "t")
					.WithSourceDecimalSeparators(true, false, "d")
					.WithTargetDecimalSeparators(false, true, "d")
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);
			numberVerifierMain.Initialize(null);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}

		[Theory]
		[InlineData("2400 bis 2483,5", "2400 to 2483.5")]
		public void NoErrorScenarios_WhenDecimalSeparatorsDifferent_LocalizationAllowed(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);
			numberVerifierMain.Initialize(null);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}

		[Theory]
		[InlineData("2,323.12,333 and 4,254.12", "2,323.12,333 and 4,254.12")]
		public void ShouldRemoveAmbiguity(string source, string target)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceThousandSeparators(comma: true, period: true)
					.WithSourceDecimalSeparators(comma: true, period: true)
					.WithTargetThousandSeparators(comma: false, period: true)
					.WithTargetDecimalSeparators(comma: true, period: false)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings: settings.Object);
			numberVerifierMain.Initialize(documentInfo: null);

			_numberValidator.GetErrors(sourceText: source, targetText: target, settings: settings.Object, sourceNumberTexts: out var sourceNumbersNormalized, targetNumberTexts: out var targetNumbersNormalized);
		}

		[Theory]
		[InlineData("1.223,3", "1.223,3")]
		public void ShouldRemoveAmbiguity_WhenSourceIsNotAmbiguous_WhenTargetIsAmbiguous(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.WithSourceThousandSeparators(true, false)
					.WithSourceDecimalSeparators(false, true)
					.WithTargetDecimalSeparators(true, true)
					.WithTargetThousandSeparators(true, true)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);
			numberVerifierMain.Initialize(null);

			_numberValidator.GetErrors(source, target, settings.Object, out var sourceNumbersNormalized, out var targetNumbersNormalized);
		}
	}
}