using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.LanguagePlatform.Core;
using System;
using System.Globalization;
using System.IO;

namespace TradosPluginTests
{
    [TestClass]
    public class XliffFileTests
    {

        /// <summary>
        /// Ensures that GetSegments will remove xliff tags and produce segments that should be identical to the
        /// segments originally passed in
        /// </summary>
        [TestMethod]
        [DeploymentItem("TaggedFile.txt")]
        public void GetSegments_TaggedText_SameAsSource()
        {
            Segment[] segments = SegmentParser.Parser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaggedFile.txt"));
            XliffConverter.xliff xliff = new XliffConverter.xliff(
                CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("fr"));
            foreach (Segment segment in segments)
                xliff.AddTranslation(segment, segment, "ETS");

            Segment[] targetSegments = xliff.GetTargetSegments();
            for (int i = 0; i < segments.Length; i++)
                Assert.AreEqual(segments[i].ToString(), targetSegments[i].ToString());
        }

        /// <summary>
        /// Ensures that if no target segment has been set, GetTargetSegments returns a null object
        /// </summary>
        [TestMethod]
        [DeploymentItem("TaggedFile.txt")]
        public void GetSegments_NoTarget_NullTargetSegments()
        {
            Segment[] segments = SegmentParser.Parser.ParseFile(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaggedFile.txt"));
            XliffConverter.xliff xliff = new XliffConverter.xliff(
                CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("fr"));
            foreach (Segment segment in segments)
                xliff.AddSourceSegment(segment);

            Segment[] targetSegments = xliff.GetTargetSegments();
            foreach (Segment segment in targetSegments)
                Assert.IsNull(segment);
        }

        /// <summary>
        /// Ensures if a null object is passed in as a segment, an exception is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddTranslation_NullSourceSegment_ThrowsException()
        {
            XliffConverter.xliff xliff = new XliffConverter.xliff(
                CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("fr"));
            xliff.AddTranslation(null, new Segment(xliff.File.SourceCulture), "ETS");
        }

        /// <summary>
        /// Ensures if a null object is passed in as a segment, an exception is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddSourceSegment_NullSourceSegment_ThrowsException()
        {
            XliffConverter.xliff xliff = new XliffConverter.xliff(
                CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("fr"));
            xliff.AddSourceSegment(null);
        }

        /// <summary>
        /// Ensures if a null object is passed in as a segment, an exception is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddSourceText_NullSourceText_ThrowsException()
        {
            XliffConverter.xliff xliff = new XliffConverter.xliff(
                CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("fr"));
            xliff.AddSourceText(null);
        }

        /// <summary>
        /// Ensures if a null object is passed in as a segment, an exception is thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AddTranslation_NullTargetSegment_ThrowsException()
        {
            XliffConverter.xliff xliff = new XliffConverter.xliff(
                CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("fr"));
            xliff.AddTranslation(new Segment(xliff.File.SourceCulture), null, "ETS");
        }

    }
}
