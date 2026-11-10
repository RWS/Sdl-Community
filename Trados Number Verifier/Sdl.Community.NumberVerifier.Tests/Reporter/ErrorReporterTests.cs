using System.Collections.Generic;
using Moq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Reporter
{
	public class ErrorReporterTests
	{
		private readonly SettingsBuilder _settingsBuilder = new();

		// CRQ-46961: the source number missing from the target sits before the end of the target's numbers
		[Theory]
		[InlineData(
			"With 60% of S&P 500 companies having reported, 82% have exceeded earnings expectations, 70% have beaten revenue estimates and third-quarter earnings are running at roughly 30% year-over-year growth.",
			"Cinq sociétés répertoriées à l’indice S&P 500 sur dix ont déjà publié leurs résultats, et 82 % d’entre elles ont fait mieux que ce à quoi les observateurs s’attendaient sur le plan des bénéfices, tandis que les revenus se sont avérés supérieurs aux estimations dans 70 % des cas. Dans l’ensemble, les bénéfices affichent une croissance de l’ordre de 30 % comparativement au même trimestre de l’exercice précédent.")]
		public void NumberRemoved_IsReported_WhenTheTargetHasNumbersAtTheSamePosition(string source, string target)
		{
			var settings = _settingsBuilder
				.AllowLocalization()
				.WithSourceDecimalSeparators(comma: false, period: true)
				.WithSourceThousandSeparators(comma: true, period: false)
				.WithTargetDecimalSeparators(comma: true, period: false)
				.WithTargetThousandSeparators(comma: false, period: false, custom: " ")
				.Build();

			var reportedMessages = new List<string>();
			var messageReporter = new Mock<IBilingualContentMessageReporter>();
			messageReporter
				.Setup(r => r.ReportMessage(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<ErrorLevel>(), It.IsAny<string>(), It.IsAny<TextLocation>(), It.IsAny<TextLocation>()))
				.Callback<object, string, ErrorLevel, string, TextLocation, TextLocation>((_, _, _, message, _, _) => reportedMessages.Add(message));

			var segmentPair = new Mock<ISegmentPair>();
			segmentPair.Setup(sp => sp.Source).Returns(new Mock<ISegment>().Object);
			segmentPair.Setup(sp => sp.Target).Returns(new Mock<ISegment>().Object);

			var numberVerifierMain = new NumberVerifierMain(settings.Object) { MessageReporter = messageReporter.Object };
			numberVerifierMain.CheckSequences(source, target, null, null, segmentPair.Object);

			Assert.Equal(new[] { PluginResources.Error_NumbersRemoved }, reportedMessages);
		}
	}
}
