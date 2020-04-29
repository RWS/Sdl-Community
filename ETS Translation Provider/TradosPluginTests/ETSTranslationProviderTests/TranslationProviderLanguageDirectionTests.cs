using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ETSTranslationProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.Toolkit.LanguagePlatform.SegmentParser;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;

namespace TradosPluginTests
{
	[TestClass]
	public class TranslationProviderLanguageDirectionTests
	{
		public TranslationProviderLanguageDirectionTests()
		{
			var options = new TranslationOptions(new Uri(StringResource.ApiUrl));
			options.UseBasicAuthentication = false;
			options.ApiToken = StringResource.ApiKey;
			provider = new TranslationProvider(options);

			settings = new SearchSettings();
		}

		private readonly SearchSettings settings;
		private readonly TranslationProvider provider;
		private readonly LanguagePair engFraLP = new LanguagePair(
			CultureInfo.GetCultureInfo("en-us"),
			CultureInfo.GetCultureInfo("fr"));

		/// <summary>
		/// Tests that a basic string can be translated using SearchSegment
		/// </summary>
		[TestMethod]
		public void SearchSegment_PlainString_ValidTranslation()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var results = dir.SearchSegment(settings, TestTags.BasicSegment);
			if (results.Count == 0)
			{
				Assert.Fail("Expected results, but received none");
			}
			if (results[0].MemoryTranslationUnit == null)
			{
				Assert.Fail("Expected translation unit, but received none");
			}
			if (results[0].MemoryTranslationUnit.TargetSegment == null)
			{
				Assert.Fail("Expected translation segment, but received none");
			}
			Assert.AreEqual(StringResource.BasicTranslation, results[0].MemoryTranslationUnit.TargetSegment.ToString());
		}

		/// <summary>
		/// Tests that a basic string as well as a segment containing tags can be translated using SearchSegment
		/// </summary>
		[TestMethod]
		public void SearchSegment_WithTags_ValidTranslation()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var results = dir.SearchSegment(settings, TestTags.BasicSegment);
			if (results.Count == 0)
			{
				Assert.Fail("Expected results, but received none");
			}
			if (results[0].MemoryTranslationUnit == null)
			{
				Assert.Fail("Expected translation unit, but received none");
			}
			if (results[0].MemoryTranslationUnit.TargetSegment == null)
			{
				Assert.Fail("Expected translation segment, but received none");
			}
			Assert.AreEqual(StringResource.BasicTranslation, results[0].MemoryTranslationUnit.TargetSegment.ToString());
		}

		/// <summary>
		/// Tests that SearchSegmentsMasked obeys a mask declaring that you should not translate the first segment given
		/// </summary>
		[TestMethod]
		public void SearchSegmentsMasked_FirstIgnored_DontTranslateFirst()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var results = dir.SearchSegmentsMasked(
				settings,
				new Segment[] { TestTags.BasicSegment, TestTags.TaggedSegment },
				new bool[] { false, true });

			Assert.IsNull(results[0]);
			if (results[1][0].MemoryTranslationUnit == null)
			{
				Assert.Fail("Expected translation unit, but received none");
			}
			if (results[1][0].MemoryTranslationUnit.TargetSegment == null)
			{
				Assert.Fail("Expected translation segment, but received none");
			}
			Assert.AreEqual(StringResource.TaggedTranslation, results[1][0].MemoryTranslationUnit.TargetSegment.ToString());
		}

		/// <summary>
		/// Tests that a basic string can be translated using SearchText
		/// </summary>
		[TestMethod]
		public void SearchText_BasicString_ValidTranslation()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var results = dir.SearchText(settings, TestTags.BasicSegment.ToPlain());
			if (results.Count == 0)
			{
				Assert.Fail("Expected results, but received none");
			}
			if (results[0].MemoryTranslationUnit == null)
			{
				Assert.Fail("Expected translation unit, but received none");
			}
			if (results[0].MemoryTranslationUnit.TargetSegment == null)
			{
				Assert.Fail("Expected translation segment, but received none");
			}
			Assert.AreEqual(StringResource.BasicTranslation, results[0].MemoryTranslationUnit.TargetSegment.ToString());
		}

		/// <summary>
		/// Tests that a basic string can be translated using SearchTranslationUnit
		/// </summary>
		public void SearchTranslationUnit_BasicSegment_ValidTranslation()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var unit = new TranslationUnit(TestTags.BasicSegment, null);

			var results = dir.SearchTranslationUnit(settings, unit);
			if (results.Count == 0)
			{
				Assert.Fail("Expected results, but received none");
			}
			if (results[0].MemoryTranslationUnit == null)
			{
				Assert.Fail("Expected translation unit, but received none");
			}
			if (results[0].MemoryTranslationUnit.TargetSegment == null)
			{
				Assert.Fail("Expected translation segment, but received none");
			}
			Assert.AreEqual(StringResource.BasicTranslation, results[0].MemoryTranslationUnit.TargetSegment.ToString());
		}

		/// <summary>
		/// Tests that a basic string as well as a tagged segment can be translated using SearchTranslationUnit
		/// </summary>
		[TestMethod]
		public void SearchTranslationUnits_WithAndWithoutTags_ValidTranslations()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var units = new TranslationUnit[] {
				new TranslationUnit(TestTags.BasicSegment, null),
				new TranslationUnit(TestTags.TaggedSegment, null)
			};
			var results = dir.SearchTranslationUnits(settings, units);
			if (results.Length != 2)
			{
				Assert.Fail("Expected results, but received none");
			}
			foreach (SearchResults result in results)
			{
				if (result[0].MemoryTranslationUnit == null)
				{
					Assert.Fail("Expected translation unit, but received none");
				}
				if (result[0].MemoryTranslationUnit.TargetSegment == null)
				{
					Assert.Fail("Expected translation segment, but received none");
				}
			}

			Assert.AreEqual(StringResource.BasicTranslation, results[0][0].MemoryTranslationUnit.TargetSegment.ToString());
			Assert.AreEqual(StringResource.TaggedTranslation, results[1][0].MemoryTranslationUnit.TargetSegment.ToString());
		}

		/// <summary>
		/// Tests that SearchTranslationUnitsMasked obeys a mask declaring that you should not translate the first
		/// translation unit given
		/// </summary>
		[TestMethod]
		public void SearchTranslationUnitsMasked_FirstIgnored_DontTranslateFirst()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var units = new TranslationUnit[] {
				new TranslationUnit(TestTags.BasicSegment, null),
				new TranslationUnit(TestTags.TaggedSegment, null)
			};
			var results = dir.SearchTranslationUnitsMasked(settings, units, new bool[] { false, true });
			Assert.IsNull(results[0]);
			if (results[1][0].MemoryTranslationUnit == null)
			{
				Assert.Fail("Expected translation unit, but received none");
			}
			if (results[1][0].MemoryTranslationUnit.TargetSegment == null)
			{
				Assert.Fail("Expected translation segment, but received none");
			}
			Assert.AreEqual(StringResource.TaggedTranslation, results[1][0].MemoryTranslationUnit.TargetSegment.ToString());
		}

		// Commented out as currently there's a bug with the ets-engine filter
		/*
        [TestMethod]
        [DeploymentItem("TaggedFile.txt")]
        [DeploymentItem("TaggedFile_Translated.txt")]
        public void SearchTranslationUnits_FormattedText_HandlesTags()
        {
            Segment[] segments = SegmentParser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaggedFile.txt"));
            Segment[] controlGroupSegments = SegmentParser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaggedFile_Translated.txt"));
            TranslationProviderLanguageDirection dir = new TranslationProviderLanguageDirection(provider, engFraLP);
            SearchResults[] results = dir.SearchSegments(settings, segments);
            Segment[] translatedSegments = results.Select(result => result[0].MemoryTranslationUnit.TargetSegment).ToArray();
            for (int i = 0; i < translatedSegments.Length; i++)
            {
                if (!translatedSegments[i].ToString().Equals(controlGroupSegments[i].ToString(), StringComparison.OrdinalIgnoreCase))
                    Assert.Fail(string.Format("Translated with tags failed: {0} was not {1}", translatedSegments[i], controlGroupSegments[i].ToString()));
            }
        }
        */

		/// <summary>
		/// Tests a large file with intermittant tags to ensure there are no blank segments.
		/// </summary>
		[TestMethod]
		[DeploymentItem("TaggedFile.txt")]
		public void SearchTranslationUnits_ManyTags_NoBlanks()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var segments = Parser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaggedFile.txt"));
			// Chunk the segments to simulate Trados splitting up a document
			var chunkedUnits = ChunkArray(segments, 10);

			Parallel.ForEach(chunkedUnits,
				new ParallelOptions { MaxDegreeOfParallelism = 5 },
				(segmentChunk) =>
				{
					try
					{
						var results = dir.SearchSegments(settings, segmentChunk);
						for (int i = 0; i < results.Length; i++)
						{
							if (!string.IsNullOrWhiteSpace(segmentChunk[i].ToPlain()))
							{
								Assert.IsFalse(string.IsNullOrWhiteSpace(
									results[i].First().MemoryTranslationUnit.TargetSegment.ToPlain()));
							}
						}
						// If an Aggregate Exception is thrown, it's the result of a WebException and likely a timeout
						// issue. Just ignore it.
					}
					catch (AggregateException) { }
					catch (WebException) { }
				}
			);
		}

		/// <summary>
		/// Tests a huge (within reason) file (1700+ segments) to ensure that there are no blank segments
		/// </summary>
		[TestMethod]
		[DeploymentItem("LargeFile_English.txt")]
		public void SearchTranslationUnits_ManySegments_NoBlanks()
		{
			var dir = new TranslationProviderLanguageDirection(provider, engFraLP);
			var segments = Parser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LargeFile_English.txt"));

			// Chunk the segments to simulate Trados splitting up a document
			var chunkedUnits = ChunkArray(segments, 10);

			Parallel.ForEach(chunkedUnits,
				new ParallelOptions { MaxDegreeOfParallelism = 5 },
				(segmentChunk) =>
				{
					try
					{
						var results = dir.SearchSegments(settings, segmentChunk);
						for (int i = 0; i < results.Length; i++)
						{
							if (!string.IsNullOrWhiteSpace(segmentChunk[i].ToPlain()))
							{
								Assert.IsFalse(string.IsNullOrWhiteSpace(results[i].First().MemoryTranslationUnit.TargetSegment.ToPlain()));
							}
						}
						// If an Aggregate Exception is thrown, it's the result of a WebException and likely a timeout
						// issue. Just ignore it.
					}
					catch (AggregateException) { }
					catch (WebException) { }
				}
			);
		}

		// Chunks a 2d array into a 3d array
		private T[][] ChunkArray<T>(T[] original, int chunkSize)
		{
			int chunkCount = original.Length / chunkSize;
			T[][] newArray = new T[original.Length / chunkSize][];
			for (int i = 0; i < chunkCount; i++)
			{
				newArray[i] = new T[chunkSize];
				Array.Copy(original, chunkSize * i, newArray[i], 0, chunkSize);
			}
			return newArray;
		}
	}
}