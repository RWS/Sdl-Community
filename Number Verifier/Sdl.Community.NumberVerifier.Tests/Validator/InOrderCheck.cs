using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Validator
{
	public class InOrderCheck
	{
		private readonly SettingsBuilder _settingsBuilder;

		public InOrderCheck()
		{
			_settingsBuilder = new SettingsBuilder();
			_settingsBuilder.ConsiderOrderOfNumbers();
		}

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

			settings.Setup(s => s.HindiNumberVerification).Returns(true);

			var numberVerifierMain = new NumberVerifierMain(settings.Object);
			

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m => Assert.Equal(PluginResources.Error_NumbersRemoved, m.ErrorMessage));
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
					.WithSourceDecimalSeparators(false, true)
					.WithSourceThousandSeparators(true, false)
					.WithTargetDecimalSeparators(false, true)
					.WithTargetThousandSeparators(true, false)
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
		[InlineData("1,234.899", "١,٢٣٤.٨٩٩")]
		public void SameSequencesDifferentMeanings_WhenDecimalSeparatorsDifferent_LocalizationAllowed(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.WithSourceThousandSeparators(comma: false, period: true)
					.WithSourceDecimalSeparators(comma: true, period: false)
					.WithTargetThousandSeparators(comma: true, period: false)
					.WithTargetDecimalSeparators(comma: false, period: true)
					.Build();

			settings.Setup(s => s.HindiNumberVerification).Returns(true);

			var numberVerifierMain = new NumberVerifierMain(settings.Object);
			

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m =>
				{
					var expectedMessage = PluginResources.NumberParser_Message_InvalidGroupSeparator;
					Assert.Contains(expectedMessage.Substring(0, expectedMessage.Length - 4), m.ErrorMessage);
				},
				m => Assert.Equal(PluginResources.Error_MissingSourceSeparators, m.ErrorMessage));
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
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.True(errorMessage.Count == 0);
		}
	}
}