using System.Collections.Generic;
using System.Linq;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Parsers.Number;
using Sdl.Community.NumberVerifier.Validator;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Validator
{
	public class CrossCheck
	{
		private readonly SettingsBuilder _settingsBuilder;

		public CrossCheck()
		{
			_settingsBuilder = new SettingsBuilder();
		}

		[Theory]
		[InlineData(" - 365")]
		public void Normalize_ReturnsCorrectValue_EvenWhenThereAreSpacesBesidesSign(string toBeNormalized)
		{
			var numberParser = new NumberParser();
			var numberToken = numberParser.Parse(toBeNormalized);

			Assert.Equal("n365", numberToken.Normalize());
		}
		
		[Theory]
		[InlineData("y225m")]
		public void NumberIsAlphanumeric_EvenWhenUnitsOfMeasurementPresent_IfStartsWithLetters(string source)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetThousandSeparators(true, false)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.GetAlphanumericList(source, true);

			Assert.Equal("y225m", errorMessage.Item2[0]);

		}
		
		[Theory]
		[InlineData("225m")]
		public void NumberIsNotAlphanumeric_WhenOnlyUnitsOfMeasurementPresent(string source)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetThousandSeparators(true, false)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.GetAlphanumericList(source, true);

			Assert.Empty(errorMessage.Item2);
		}
		
		[Theory]
		[InlineData("z-3")]
		public void NonNumbersIgnoredByNumberConsistencyChecker(string source)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetThousandSeparators(true, false)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, null);

			Assert.Empty(errorMessage);
		}

		//[Theory]
		//[InlineData("11,200", null)]
		//public void NumberRemoved_WhenNumberIsNotPresentInTheTarget(string source, string target)
		//{
		//	var settings =
		//		_settingsBuilder
		//			.RequireLocalization()
		//			.WithSourceDecimalSeparators(true, false)
		//			.WithTargetThousandSeparators(true, false)
		//			.Build();

		//	var numberVerifierMain = new NumberVerifierMain(settings.Object);

		//	var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

		//	Assert.Collection(errorMessage,
		//		m => Assert.Equal(PluginResources.Error_NumbersRemoved, m.ErrorMessage));
		//}

		//Segment-pair-level comparison scenarios
		[Theory]
		[InlineData("11,200", null)]
		public void NumberRemoved_WhenNumberIsNotPresentInTheTarget(string source, string target)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetThousandSeparators(true, false)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m => Assert.Equal(PluginResources.Error_NumbersRemoved, m.ErrorMessage));
		}
		
		//TODO:Add some tests with more than one instance of a thousand sep

		[Theory]
		[InlineData("1 554,5 some word 1.234,5 another word -1,222,3", "1.554,5 test 1,234,5 another test word −1.222,3")]
		public void ThousandsSeparatorsCommaPeriod(string source, string target)
		{
			var settings = _settingsBuilder
				.RequireLocalization()
				.WithTargetDecimalSeparators(comma: true, period: false)
				.WithSourceThousandSeparators(comma: true, period: true, custom: " ")
				.WithSourceDecimalSeparators(comma: true, period: false)
				.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target).Where(e=>e.ErrorType == NumberVerifier.Validator.NumberText.ErrorLevel.SegmentPairLevel);

			var expectedErrors = new List<string>
			{
				PluginResources.Error_DifferentValues,
				PluginResources.Error_DifferentSequences,
				PluginResources.Error_DifferentSequences,
				PluginResources.Error_NumberAdded,
				PluginResources.Error_NumberAdded
			};

			Assert.Equal(expectedErrors, errorMessage.Select(em => em.ErrorMessage));

		}

		[Theory]
		[InlineData(null, "11,200")]
		public void NumberAdded_WhenNumberIsNotPresentInTheSource(string source, string target)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetThousandSeparators(true, false)
					.Build();

			settings.Setup(s => s.HindiNumberVerification).Returns(true);

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m => Assert.Equal(PluginResources.Error_NumberAdded, m.ErrorMessage));
		}

		[Theory]
		[InlineData("343,44", "343.44")]
		public void DifferentSequences_WhenSequencesAreDifferent(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.WithSourceDecimalSeparators(comma: false, period: true)
					.WithSourceThousandSeparators(comma: true, period: false)
					.WithTargetDecimalSeparators(comma: false, period: true)
					.WithTargetThousandSeparators(comma: true, period: false)
					.Build();

			settings.Setup(s => s.HindiNumberVerification).Returns(true);

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m =>
				{
					var expectedMessage = PluginResources.NumberParser_Message_TheGroupValidIsOutOfRange;
					Assert.Contains(expectedMessage.Substring(0, expectedMessage.Length - 4), m.ErrorMessage);
				},
				m => Assert.Equal(PluginResources.Error_DifferentSequences, m.ErrorMessage));
		}

		[Theory]
		[InlineData("34,2", "11,200")]
		public void DifferentValues_WhenNumbersAreDifferent(string source, string target)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetThousandSeparators(true, false)
					.Build();

			settings.Setup(s => s.HindiNumberVerification).Returns(true);

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m => Assert.Equal(PluginResources.Error_DifferentValues, m.ErrorMessage));
		}

		[Theory]
		[InlineData("11,200.300", "11,200.300")]
		[InlineData("1,234.890", "١,٢٣٤.٨٩٠")]
		public void SameSequencesDifferentMeanings_WhenDecimalSeparatorsDifferent_LocalizationAllowed(string source, string target)
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

			var numberVerifierMain = new NumberVerifierMain(settings.Object);
			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m =>
				{
					var expectedMessage = PluginResources.NumberParser_Message_InvalidGroupSeparator;
					m.ErrorMessage.Contains(expectedMessage.Substring(0, expectedMessage.Length - 4));
				},
				m => Assert.Equal(m.ErrorMessage, PluginResources.Error_MissingSourceSeparators));
		}

		[Theory]
		[InlineData("11,200", "11,200")]
		public void SameSequencesButDifferentValues_WhenSeparatorsAreInSamePlacesButHaveDifferentMeanings(string source, string target)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetThousandSeparators(true, false)
					.Build();

			settings.Setup(s => s.HindiNumberVerification).Returns(true);

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m => Assert.Equal(PluginResources.Error_SameSequenceDifferentValues, m.ErrorMessage));
		}

		[Theory]
		[InlineData("11,20", "11,20")]
		public void NumberUnlocalised_WhenSeparatorsAreInSamePlacesButHaveDifferentMeanings_AndRoleOfSeparatorInTheTargetAtThatPositionIsNotValid(string source, string target)
		{
			var settings =
				_settingsBuilder
					.RequireLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetThousandSeparators(true, false)
					.Build();

			settings.Setup(s => s.HindiNumberVerification).Returns(true);

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m => Assert.Equal(PluginResources.Error_MissingTargetSeparators, m.ErrorMessage),
				m =>
				{
					var expectedMessage = PluginResources.NumberParser_Message_TheGroupValidIsOutOfRange;
					Assert.Contains(expectedMessage.Substring(0, expectedMessage.Length - 4), m.ErrorMessage);
				});
		}

		//Other scenarios
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

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}

		[Theory]
		[InlineData("2400 bis 2483,5", "2400 to 2483.5")]
		public void NoErrors_WhenDecimalSeparatorsDifferent_LocalizationAllowed(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.WithSourceDecimalSeparators(true, false)
					.WithTargetDecimalSeparators(false, true)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}
	}
}
