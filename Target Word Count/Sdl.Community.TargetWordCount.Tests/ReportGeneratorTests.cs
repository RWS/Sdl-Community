using System.Collections.Generic;
using System.ComponentModel;
using NSubstitute;
using Sdl.Community.TargetWordCount.Models;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.Core;
using Xunit;
using Xunit.Abstractions;

namespace Sdl.Community.TargetWordCount.Tests
{
	public class ReportGeneratorTests
	{
		private readonly ITestOutputHelper output;

		public ReportGeneratorTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]
		public void GenerateCreatesReport()
		{
			// Arrange
			var counter = Substitute.For<ISegmentWordCounter>();
			counter.FileName.Returns("Some file name.sdlxliff");

			var countData = CountData.Create(10, 10, 10, 10, 10);
			var segCountInfo1 = CreateSegmentCountInfo(countData, true);
			var segCountInfo2 = CreateSegmentCountInfo(countData, matchPercent: 100);
			var segCountInfo3 = CreateSegmentCountInfo(countData, matchPercent: 95);
			var segCountInfo4 = CreateSegmentCountInfo(countData, matchPercent: 85);
			var segCountInfo5 = CreateSegmentCountInfo(countData, matchPercent: 75);
			var segCountInfo6 = CreateSegmentCountInfo(countData, matchPercent: 55);
			var segCountInfo7 = CreateSegmentCountInfo(countData, matchPercent: 45);

			var languages = new Language[] { new Language("ja-JP"), new Language("en-US") };

			var fileCountInfo = new FileCountInfo(new List<SegmentCountInfo>() { segCountInfo1,
																				 segCountInfo2,
																				 segCountInfo3,
																				 segCountInfo4,
																				 segCountInfo5,
																				 segCountInfo6,
																				 segCountInfo7
																			   },
												  languages,
												  Substitute.For<IRepetitionsTable>());

			counter.FileCountInfo.Returns(fileCountInfo);

			var counters = new List<ISegmentWordCounter>()
			{
				counter
			};

			var settings = Substitute.For<IWordCountBatchTaskSettings>();
			settings.InvoiceRates = new BindingList<InvoiceItem>()
			{
				new InvoiceItem(RateType.PerfectMatch, "1.25"),
				new InvoiceItem(RateType.ContextMatch, "10"),
				new InvoiceItem(RateType.Repetitions, "10"),
				new InvoiceItem(RateType.CrossFileRepetitions, "10"),
				new InvoiceItem(RateType.OneHundred, "1.50"),
				new InvoiceItem(RateType.NinetyFive, "10"),
				new InvoiceItem(RateType.EightyFive, "10"),
				new InvoiceItem(RateType.SeventyFive, "10"),
				new InvoiceItem(RateType.Fifty, "10"),
				new InvoiceItem(RateType.New, "10")
			};

			settings.ReportLockedSeperately = false;
			settings.UseSource = true;
			settings.Culture = "English";

			// Act
			var root = ReportGenerator.Generate(counters, settings);

			// Assert
			Assert.NotNull(root);
			output.WriteLine(root);
		}

		private SegmentCountInfo CreateSegmentCountInfo(CountData countdata,
														bool isRepeated = false,
														byte matchPercent = 0)
		{
			var countData = countdata;
			var transOrigin = Substitute.For<ITranslationOrigin>();

			if (isRepeated)
			{
				transOrigin.IsRepeated.Returns(true);
			}

			transOrigin.MatchPercent = matchPercent;

			return new SegmentCountInfo(transOrigin,
										countData,
										false,
										0);
		}
	}
}