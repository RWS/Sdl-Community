using System.Globalization;
using System.Text.RegularExpressions;
using NSubstitute;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.TranslationMemory;
using Xunit;

namespace Sdl.Community.Qualitivity.Tests
{
	// Expected counts are what Trados.Community.Toolkit.LanguagePlatform 6.1.0 returned for the same segments
	public class SegmentWordCounterTests
	{
		private static readonly IDocumentItemFactory Factory;

		static SegmentWordCounterTests()
		{
			StudioAssemblyResolver.Install();
			Factory = DefaultDocumentItemFactory.CreateInstance();
		}

		[Fact]
		public void Count_PlainText_CountsWordsAndCharacters()
		{
			var counts = Count("Terminology project check editor update device improve editor screen version update.");

			Assert.Equal(new[] { 11, 74 }, new[] { counts.Words, counts.Characters });
		}

		[Theory]
		[InlineData("Press the {b}Start{/b} button to begin.", 6, 27)]
		[InlineData("Insert the battery{x} and close the cover{x}.", 7, 33)]
		public void Count_TagsAndPlaceholders_CountsThemAsTagsAndPlaceables(string source, int words, int characters)
		{
			var counts = Count(source);

			Assert.Equal(new[] { words, characters, 2, 2 }, new[] { counts.Words, counts.Characters, counts.Tags, counts.Placeables });
		}

		[Theory]
		[InlineData("fast - simple", 2, 11)]
		[InlineData("A state-of-the-art, well-known solution - fast and simple.", 7, 51)]
		public void Count_DashBetweenSpaces_SeparatesWords(string source, int words, int characters)
		{
			var counts = Count(source);

			Assert.Equal(new[] { words, characters }, new[] { counts.Words, counts.Characters });
		}

		[Theory]
		[InlineData("The device weighs 3.5 kg and costs $1,234.56.", 7, 39, 2)]
		[InlineData("Released on 12/03/2024 at 14:30, version 7.0.3.0.", 7, 43, 3)]
		[InlineData("Visit https://www.rws.com or write to support@example.com.", 6, 53, 2)]
		[InlineData("Set the value to 20% (approx. 1/4) of max.", 10, 34, 3)]
		[InlineData("ISO 9001:2015 and EN-60950 compliance, part no. AB-1234-X.", 9, 51, 5)]
		[InlineData("Temperature range: -20 °C to +45 °C; 12 V DC.", 7, 39, 4)]
		[InlineData("NASA, the U.S.A. and e.g. etc. are abbreviations.", 8, 42, 1)]
		public void Count_RecognizedTokens_CountsThemAsPlaceables(string source, int words, int characters, int placeables)
		{
			var counts = Count(source);

			Assert.Equal(new[] { words, characters, placeables }, new[] { counts.Words, counts.Characters, counts.Placeables });
		}

		// The toolkit copies the target into an empty source before counting
		[Fact]
		public void Count_EmptySource_CountsTarget()
		{
			var counts = Count(string.Empty, target: "Press the {b}Start{/b} button to begin.");

			Assert.Equal(new[] { 6, 27, 2, 2 }, new[] { counts.Words, counts.Characters, counts.Tags, counts.Placeables });
		}

		[Theory]
		[InlineData("ja-JP", "ファイルは自動的に保存されます。", 8, 16, 0, 0)]
		[InlineData("ja-JP", "{b}開始{/b}ボタンを押してください。", 6, 14, 2, 2)]
		[InlineData("ja-JP", "重量は3.5kgです。", 4, 11, 0, 1)]
		[InlineData("zh-CN", "文件将自动保存。", 4, 8, 0, 0)]
		[InlineData("zh-CN", "请按{b}开始{/b}按钮。", 4, 7, 2, 2)]
		[InlineData("de-DE", "Gewicht: 3,5 kg; Preis: 1.234,56 €.", 4, 32, 0, 2)]
		public void Count_OtherSourceLanguages_CountsLikeTheTm(string language, string source, int words, int characters, int tags, int placeables)
		{
			var counts = Count(source, language: language);

			Assert.Equal(new[] { words, characters, tags, placeables }, new[] { counts.Words, counts.Characters, counts.Tags, counts.Placeables });
		}

		[Theory]
		[InlineData("Do not {l}remove the cover{/l} now.", 3, 9, 1, 1)]
		[InlineData("Press {b}Start{/b} and {l}wait{/l}.", 3, 14, 3, 3)]
		public void Count_LockedContent_CountsItAsOneTagNotAsWords(string source, int words, int characters, int tags, int placeables)
		{
			var counts = Count(source);

			Assert.Equal(new[] { words, characters, tags, placeables }, new[] { counts.Words, counts.Characters, counts.Tags, counts.Placeables });
		}

		// The toolkit counts the text of insertions and skips the text of deletions
		[Theory]
		[InlineData("Press {d}the red {/d}Start button.", "Press Start button.")]
		[InlineData("Press {i}the {/i}Start button.", "Press the Start button.")]
		public void Count_TrackedChanges_CountsLikeTheToolkit(string source, string counted)
		{
			var counts = Count(source);
			var expected = Count(counted);

			Assert.Equal(new[] { expected.Words, expected.Characters, expected.Tags, expected.Placeables }, new[] { counts.Words, counts.Characters, counts.Tags, counts.Placeables });
		}

		private static WordCounts Count(string source, string target = "", string language = "en-US")
		{
			// Counts depend on the source language only; the TM still needs a target
			return new SegmentWordCounter(new CultureInfo(language), new CultureInfo("fr-FR")).Count(Pair(source, target));
		}

		private static ISegmentPair Pair(string sourceText, string targetText)
		{
			var segmentPair = Substitute.For<ISegmentPair>();
			segmentPair.Source.Returns(Segment(sourceText));
			segmentPair.Target.Returns(Segment(targetText));
			return segmentPair;
		}

		// "{b}…{/b}" is a tag pair, "{x}" a placeholder, "{l}…{/l}" locked content, "{i}…{/i}" / "{d}…{/d}" a tracked insertion / deletion
		private static ISegment Segment(string spec)
		{
			var segment = Factory.CreateSegment(Factory.CreateSegmentPairProperties());
			IAbstractMarkupDataContainer container = segment;
			var tagId = 1;
			foreach (var part in Regex.Split(spec, @"(\{/?[bxlid]\})"))
			{
				switch (part)
				{
					case "": break;
					case "{x}":
						var placeholder = Factory.PropertiesFactory.CreatePlaceholderTagProperties("<x/>");
						placeholder.TagId = new TagId((tagId++).ToString());
						container.Add(Factory.CreatePlaceholderTag(placeholder));
						break;
					case "{b}":
						var start = Factory.PropertiesFactory.CreateStartTagProperties("<b>");
						start.TagId = new TagId((tagId++).ToString());
						var tagPair = Factory.CreateTagPair(start, Factory.PropertiesFactory.CreateEndTagProperties("</b>"));
						container.Add(tagPair);
						container = tagPair;
						break;
					case "{l}":
						var locked = Factory.CreateLockedContent(Factory.PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual));
						container.Add(locked);
						container = locked.Content;
						break;
					case "{i}":
					case "{d}":
						var revision = Factory.CreateRevision(Factory.PropertiesFactory.CreateRevisionProperties(part == "{i}" ? RevisionType.Insert : RevisionType.Delete));
						container.Add(revision);
						container = revision;
						break;
					case "{/b}":
					case "{/l}":
					case "{/i}":
					case "{/d}":
						container = segment;
						break;
					default:
						container.Add(Factory.CreateText(Factory.PropertiesFactory.CreateTextProperties(part)));
						break;
				}
			}
			return segment;
		}
	}
}
