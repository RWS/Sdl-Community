namespace Trados2007.UnitTests
{
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemory;
    using Sdl.TranslationStudio.Plugins.Trados2007;

    [TestClass()]
    public class SegmentParserTest
    {
        [TestMethod()]
        public void SegmentParser_CreateSegment_GeneralAndPlainString()
        {
            ScoringResult scoringResult = new ScoringResult();
            SearchSettings searchSettings = new SearchSettings();
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            string matchResult = "some plain string";

            var actual = SegmentParser.CreateSegment(scoringResult, searchSettings, matchResult, culture);

            Assert.AreEqual(CultureInfo.GetCultureInfo("en-US"), actual.Culture);
            Assert.AreEqual("en-US", actual.CultureName);
            Assert.AreEqual(1, actual.Elements.Count);
            Assert.IsFalse(actual.HasPairedTags);
            Assert.IsFalse(actual.HasPlaceables);
            Assert.IsFalse(actual.HasTags);
            Assert.IsFalse(actual.IsEmpty);
            Assert.IsTrue(actual.IsValid());
            Assert.AreEqual("some plain string", actual.ToPlain());
            Assert.AreEqual("some plain string", actual.ToString());
        }

        [TestMethod()]
        public void SegmentParser_CreateSegment_OneStartTagWithoutEscaped()
        {
            ScoringResult scoringResult = new ScoringResult();
            SearchSettings searchSettings = new SearchSettings();
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            string matchResult = "<ut Type=\"start\" RightEdge=\"angle\">Effectively managing employee meals means balancing profit objectives with people objectives.";

            var actual = SegmentParser.CreateSegment(scoringResult, searchSettings, matchResult, culture);

            Assert.IsTrue(actual.ToString().Contains("Effectively managing employee meals means balancing profit objectives with people objectives."));
            Assert.AreEqual("Effectively managing employee meals means balancing profit objectives with people objectives.", actual.ToPlain());
        }

        [TestMethod()]
        public void SegmentParser_CreateSegment_WithEscaped()
        {
            ScoringResult scoringResult = new ScoringResult();
            SearchSettings searchSettings = new SearchSettings();
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            string matchResult = "<ut Type=\"start\" RightEdge=\"angle\">&lt;f&quot;TimesNewRomanPSMT&quot;&gt;</ut>Effectively managing employee meals means balancing profit objectives with people objectives.";

            var actual = SegmentParser.CreateSegment(scoringResult, searchSettings, matchResult, culture);

            Assert.AreEqual("<f\"TimesNewRomanPSMT\">Effectively managing employee meals means balancing profit objectives with people objectives.", actual.ToPlain());
        }

        [TestMethod()]
        public void SegmentParser_UnescapeLiterals_1()
        {
            string escaped = "some &amp; text with &lt;pseudo-tags&gt;";

            var result = SegmentParser.UnescapeLiterals(escaped);

            Assert.AreEqual(@"some & text with <pseudo-tags>", result);
        }

        [TestMethod()]
        public void SegmentParser_UnescapeLiterals_2()
        {
            string escaped = "<ut Type=\"start\" RightEdge=\"angle\">&lt;f&quot;TimesNewRomanPSMT&quot;&gt;</ut>Effectively managing employee meals means balancing profit objectives with people objectives.";

            var result = SegmentParser.UnescapeLiterals(escaped);

            Assert.AreEqual("<ut Type=\"start\" RightEdge=\"angle\"><f\"TimesNewRomanPSMT\"></ut>Effectively managing employee meals means balancing profit objectives with people objectives.", result);
        }

        [TestMethod()]
        public void SegmentParser_GetTag_StandaloneTag()
        {
            string value = "<ut Type=\"start\" RightEdge=\"angle\"/>";

            SegmentElement actual = SegmentParser.GetTag(value);

            Assert.AreEqual(TagType.Standalone, (actual as Tag).Type);
            Assert.AreEqual(@"ut", (actual as Tag).TagID);
            Assert.IsNotNull((actual as Tag).Anchor);
        }
    }
}
