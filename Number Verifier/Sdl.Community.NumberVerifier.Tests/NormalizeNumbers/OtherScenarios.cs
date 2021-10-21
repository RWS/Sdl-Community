using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class OtherScenarios
	{
		private readonly SettingsBuilder _settingsBuilder;

		public OtherScenarios()
		{
			_settingsBuilder = new SettingsBuilder();
		}

		[Theory]
		[InlineData("2400 bis 2483,5", "2400 to 2483.5")]
		[InlineData("11t200d300", "11t200d300")]
		public void NoErrorScenarios_WhenDecimalSeparatorsDifferent_LocalizationAllowed(string source, string target)
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
		[InlineData("11,200.300", "11,200.300")]
		public void ErrorScenarios_WhenDecimalSeparatorsDifferent_LocalizationAllowed(string source, string target)
		{
			var settings =
				_settingsBuilder
					.AllowLocalization()
					.WithSourceThousandSeparators(false, true)
					.WithTargetThousandSeparators(true, false)
					.WithSourceDecimalSeparators(true, false)
					.WithTargetDecimalSeparators(false, true)
					.Build();

			var numberVerifierMain = new NumberVerifierMain(settings.Object);
			numberVerifierMain.Initialize(null);

			var errorMessage = numberVerifierMain.CheckSourceAndTarget(source, target);

			Assert.Collection(errorMessage,
				m => Assert.Equal(PluginResources.Error_NumbersNotIdentical, m.ErrorMessage),
				m => Assert.Equal(PluginResources.Error_NumbersRemoved, m.ErrorMessage)
				);
		}
	}
}