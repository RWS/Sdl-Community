using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.Toolkit.LanguagePlatform.SegmentParser;
using Sdl.LanguagePlatform.Core;

namespace TradosPluginTests
{
	[TestClass]
	public class SegmentParserTests
	{
		private const string startingTagTemplate = "<1 id={0}>";
		private const string standaloneTagTemplate = "<1 id={0}/>";
		private const string placeholderTagTemplate = @"<1 id={0} text-equiv=""{1}""/>";

		/// <summary>
		/// Verifies parsing a file complete with tags will complete correctly and have the proper tags/segments
		/// </summary>
		[TestMethod]
		[DeploymentItem("TaggedFile.txt")]
		public void ParseFile_TaggedFile_NoErrors()
		{
			var segments = Parser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaggedFile.txt"));
			Assert.AreEqual(139, segments.Length);
			Assert.AreEqual(9, segments[0].Elements.Count);
			Assert.AreEqual(5, segments[0].GetTagCount());
			Assert.AreEqual(false, segments[0].HasUnmatchedStartOrEndTags());
			Assert.IsNull(segments[0].FindTag(TagType.Standalone, 43));
			Assert.IsNotNull(segments[0].FindTag(TagType.Standalone, 3));
		}

		/// <summary>
		/// Verifies parsing a non-existent file will throw an exception
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void ParseFile_NoFile_Exception()
		{
			var segments = Parser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NonexistentFile.txt"));
		}

		/// <summary>
		/// Verifies parsing a simple line produces a very simple segment
		/// </summary>
		[TestMethod]
		public void ParseLine_NoTags_ParsesText()
		{
			const string text = @"Once downloaded, open the .sdlplugin file and the SDL Trados Studio plugin installer will launch.";

			var segment = Parser.ParseLine(text);
			Assert.IsFalse(segment.HasTags);
			Assert.AreEqual(text, segment.ToPlain());
		}

		/// <summary>
		/// Verifies parsing a line with an unmatched starting tag will throw an exception
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void ParseLine_UnmatchedBeginningTag_ThrowsError()
		{
			const string text = "Tick the box to accept <1 id=217>Terms and Conditions and click <2 id=220>Continue</2>.";
			var segment = Parser.ParseLine(text);
		}

		/// <summary>
		/// Verifies parsing a line with an unmatched ending tag will throw an exception
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void ParseLine_UnmatchedEndingTag_ThrowsError()
		{
			const string text = "Tick the box to accept Terms and Conditions</1> and click <2 id=220>Continue</2>.";
			var segment = Parser.ParseLine(text);
		}

		/// <summary>
		/// Verifies that parsing a line containing tags will correctly create populate the segment with the tags
		/// </summary>
		[TestMethod]
		public void ParseLine_Tagged_ParsesText()
		{
			const string text = "Tick the box to accept <1 id=217>Terms and Conditions</1> and click <2 id=220>Continue</2>.";
			var segment = Parser.ParseLine(text);
			Assert.AreEqual(9, segment.Elements.Count);
			Assert.AreEqual(2, segment.GetTagCount());
			Assert.AreEqual(false, segment.HasUnmatchedStartOrEndTags());
			Assert.IsNull(segment.FindTag(TagType.Start, 217));
			Assert.IsNotNull(segment.FindTag(TagType.Start, 1));
		}

		/// <summary>
		/// Verifies that if we're parsing a tag whose text equivalence has a tag within it
		/// (eg <1 id=1 text-equiv="<code class="prettyprint">master-host</code>""/>
		/// that we're able to realize that the text equivalent is the entire code tag.
		///
		/// We also test with tags that contain various symbols including a quote.
		/// </summary>
		[TestMethod]
		public void ParseLine_ContainsNestedTag_CreatesOneTag()
		{
			const string text = @"For the purposes of this documentation, we will reference an example server running
                                at the host <1 id=1 text-equiv=""<code class=""prettyprint"">master-host</code>""/>
                                and port <2 id=2 text-equiv=""<code class=""prettyprint"">8001</code>""/>, with HTTPS
                                enabled, so the <3 id=25>base URL</3> for the REST API will be:";

			var segment = Parser.ParseLine(text);
			Assert.IsTrue(segment.Elements[1] is Tag);
			Assert.AreEqual(@"<code class=""prettyprint"">master-host</code>", ((Tag)segment.Elements[1]).TextEquivalent);

			const string nestedQuotes = @"Må ikke indeholde nogen af følgende tegn: <1 id=1 text-equiv=""<code>/" +
										 @"</code>""/><2 id=2 text-equiv=""<code>""</code>""/><3 id=3 text-equiv=""" +
										 @"<code>,</code>""/><4 id=4 text-equiv=""<code>%</code>""/>" +
										 @"<5 id=5 text-equiv=""<code>+</code>""/> ";

			segment = Parser.ParseLine(nestedQuotes);
			Assert.IsTrue(segment.Elements[1] is Tag);
			Assert.AreEqual(@"<code>/</code>", ((Tag)segment.Elements[1]).TextEquivalent);
			Assert.IsTrue(segment.Elements[2] is Tag);
			Assert.AreEqual(@"<code>""</code>", ((Tag)segment.Elements[2]).TextEquivalent);
			Assert.IsTrue(segment.Elements[3] is Tag);
			Assert.AreEqual(@"<code>,</code>", ((Tag)segment.Elements[3]).TextEquivalent);
			Assert.IsTrue(segment.Elements[4] is Tag);
			Assert.AreEqual(@"<code>%</code>", ((Tag)segment.Elements[4]).TextEquivalent);
			Assert.IsTrue(segment.Elements[5] is Tag);
			Assert.AreEqual(@"<code>+</code>", ((Tag)segment.Elements[5]).TextEquivalent);
		}

		/// <summary>
		/// Verifying that if we try to parse a tag/string that isn't a tag, it'll return null
		/// </summary>
		[TestMethod]
		public void ParseTag_NoTag_Null()
		{
			const string text = @"Once downloaded, open the .sdlplugin file and the SDL Trados Studio plugin installer will launch.";
			Assert.IsNull(Parser.ParseTag(text));
		}

		/// <summary>
		/// Tests that you can have any character (except spaces, slashes and greater-than signs) in a tag's id
		/// </summary>
		[TestMethod]
		public void ParseTag_AbnormalID_IDCorrectlyParsed()
		{
			const string basicTagID = @"p1.11";
			Assert.AreEqual(basicTagID, Parser.ParseTag(string.Format(startingTagTemplate, basicTagID)).TagID);
			Assert.AreEqual(basicTagID, Parser.ParseTag(string.Format(standaloneTagTemplate, basicTagID)).TagID);
			Assert.AreEqual(basicTagID, Parser.ParseTag(string.Format(placeholderTagTemplate, basicTagID, "cat")).TagID);

			const string intricateTagID = @"abc123,.;'[]\-={}|:""<?";
			Assert.AreEqual(intricateTagID, Parser.ParseTag(string.Format(startingTagTemplate, intricateTagID)).TagID);
			Assert.AreEqual(intricateTagID, Parser.ParseTag(string.Format(standaloneTagTemplate, intricateTagID)).TagID);
			Assert.AreEqual(intricateTagID, Parser.ParseTag(string.Format(placeholderTagTemplate, intricateTagID, "cat")).TagID);
		}

		[TestMethod]
		public void ParseTag_XAttribute_IDCorrectlyParsed()
		{
			const string id = "456";
			const int xAttribute = 123;

			string startingTagWithXAttr = string.Format("<1 x={0} id={1}>", xAttribute, id);
			string standaloneTagWithXAttr = string.Format("<1 x={0} id={1}/>", xAttribute, id);
			string placeholderTagWithXAttr = string.Format(@"<1 x={0} id={1} text-equiv=""abc""/>", xAttribute, id);
			Assert.AreEqual(id, Parser.ParseTag(startingTagWithXAttr).TagID);
			Assert.AreEqual(id, Parser.ParseTag(standaloneTagWithXAttr).TagID);
			Assert.AreEqual(id, Parser.ParseTag(placeholderTagWithXAttr).TagID);
		}

		/// <summary>
		/// Test that if there's a space or slash in the id, the tag isn't parsed and returns null
		/// </summary>
		[TestMethod]
		public void ParseTag_IllegalID_NullTag()
		{
			const string idWithSpace = @"abc 123";
			Assert.IsNull(Parser.ParseTag(string.Format(startingTagTemplate, idWithSpace)));
			Assert.IsNull(Parser.ParseTag(string.Format(standaloneTagTemplate, idWithSpace)));
			Assert.IsNull(Parser.ParseTag(string.Format(placeholderTagTemplate, idWithSpace, "cat")));

			const string idWithSlash = @"abc/123";
			Assert.IsNull(Parser.ParseTag(string.Format(startingTagTemplate, idWithSlash)));
			Assert.IsNull(Parser.ParseTag(string.Format(standaloneTagTemplate, idWithSlash)));
			Assert.IsNull(Parser.ParseTag(string.Format(placeholderTagTemplate, idWithSlash, "cat")));

			// Testing with greater-than is tricky since if it has that, we parse it as a starting tag, then assume
			// the text after is part of the next Tag. e.g.:
			// <1 id=abc>123> can be read as the tag being <1 id=abc> and the text tag afterward being "123>". For now,
			// then, let's abstain from trying to test for this case.
		}

		/// <summary>
		/// Verifies that parsing each tag returns the proper tag type
		/// </summary>
		[TestMethod]
		public void ParseTag_AllTags_NoNulls()
		{
			Assert.AreEqual(TagType.Start, Parser.ParseTag(string.Format(startingTagTemplate, "123")).Type);
			Assert.AreEqual(TagType.End, Parser.ParseTag("</1>").Type);
			Assert.AreEqual(TagType.Standalone, Parser.ParseTag(string.Format(standaloneTagTemplate, "123")).Type);
			Assert.AreEqual(TagType.LockedContent, Parser.ParseTag(
				string.Format(placeholderTagTemplate,
				"123", @"""<code class=""prettyprint"">8001</code>""/>")).Type);
		}
	}
}