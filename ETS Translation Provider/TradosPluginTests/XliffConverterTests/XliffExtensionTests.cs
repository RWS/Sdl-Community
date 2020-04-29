using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.Toolkit.LanguagePlatform.SegmentParser;
using Sdl.Community.Toolkit.LanguagePlatform.XliffConverter;
using Sdl.LanguagePlatform.Core;

namespace TradosPluginTests
{
	[TestClass]
	public class XliffExtensionTests
	{
		/// <summary>
		/// Verifies that ToXliffString will correctly encode Trados tags into xliff tags
		/// </summary>
		[TestMethod]
		public void ToXliffString_NoTags_Converts()
		{
			const string text = @"For the purposes of this documentation, we will " +
								@"reference an example server running at the host <1 id=1 text-equiv=""<code " +
								@"class=""prettyprint"">master-host</code>""/> and port <2 id=2 " +
								@"text-equiv=""<code class=""prettyprint"">8001</code>""/>, with HTTPS " +
								@"enabled, so the <3 id=25>base URL</3> for the REST API will be:";

			const string xliffText = @"For the purposes of this documentation, we will reference an example server " +
									 @"running at the host <x id=""1"">&lt;1 id=1 text-equiv=""&lt;code " +
									 @"class=""prettyprint"">master-host&lt;/code>""/></x> and port " +
									 @"<x id=""2"">&lt;2 id=2 text-equiv=""&lt;code class=""prettyprint"">" +
									 @"8001&lt;/code>""/></x>, with HTTPS enabled, so the <bpt id=""25"">&lt;3 " +
									 @"id=25></bpt>base URL<ept id=""25"">&lt;/3></ept> for the REST API will be:";

			var segment = Parser.ParseLine(text);
			Assert.AreEqual(xliffText, segment.ToXliffString());

			segment = Parser.ParseLine("For the purposes of this documentation, <PROJECT> is a project");

			Assert.AreEqual("For the purposes of this documentation, &lt;PROJECT> is a project",
				segment.ToXliffString());
		}
	}
}