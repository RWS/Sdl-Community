using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.MTEdge.Provider.XliffConverter.Converter;
using Sdl.Community.MTEdge.Provider.XliffConverter.SegmentParser;
using Sdl.LanguagePlatform.Core;
using File = Sdl.Community.MTEdge.Provider.XliffConverter.Models.File;

namespace Sdl.Community.MTEdge.UnitTests.XliffConverterTests
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
			var segments = Parser.ParseFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaggedFile.txt"));

			var file = new File
			{
				SourceCulture = CultureInfo.GetCultureInfo("en-us"),
				TargetCulture = CultureInfo.GetCultureInfo("fr")
			};

			var xliff = new Xliff
			{
				File = file
			};


			foreach (var segment in segments)
			{
				xliff.AddTranslation(segment, segment, "MTEdge");
			}

			var targetSegments = xliff.GetTargetSegments();
			for (int i = 0; i < segments.Length; i++)
			{
				Assert.AreEqual(segments[i].ToString(), targetSegments[i].ToString());
			}
		}

		/// <summary>
		/// Ensures that if no target segment has been set, GetTargetSegments returns a null object
		/// </summary>
		[TestMethod]
		[DeploymentItem("TaggedFile.txt")]
		public void GetSegments_NoTarget_NullTargetSegments()
		{
			var segments = Parser.ParseFile(
				Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TaggedFile.txt"));

			var file = new File
			{
				SourceCulture = CultureInfo.GetCultureInfo("en-us"),
				TargetCulture = CultureInfo.GetCultureInfo("fr")
			};

			var xliff = new Xliff
			{
				File = file
			};

			foreach (var segment in segments)
			{
				xliff.AddSourceSegment(segment);
			}

			var targetSegments = xliff.GetTargetSegments();
			foreach (var segment in targetSegments)
			{
				Assert.IsNull(segment);
			}
		}

		/// <summary>
		/// Ensures if a null object is passed in as a segment, an exception is thrown.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void AddTranslation_NullSourceSegment_ThrowsException()
		{
			var file = new File
			{
				SourceCulture = CultureInfo.GetCultureInfo("en-us"),
				TargetCulture = CultureInfo.GetCultureInfo("fr")
			};

			var xliff = new Xliff
			{
				File = file
			};

			xliff.AddTranslation(null, new Segment(xliff.File.SourceCulture), "MTEdge");
		}

		/// <summary>
		/// Ensures if a null object is passed in as a segment, an exception is thrown.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void AddSourceSegment_NullSourceSegment_ThrowsException()
		{
			var file = new File
			{
				SourceCulture = CultureInfo.GetCultureInfo("en-us"),
				TargetCulture = CultureInfo.GetCultureInfo("fr")
			};

			var xliff = new Xliff
			{
				File = file
			};

			xliff.AddSourceSegment(null);
		}

		/// <summary>
		/// Ensures if a null object is passed in as a segment, an exception is thrown.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void AddSourceText_NullSourceText_ThrowsException()
		{
			var file = new File
			{
				SourceCulture = CultureInfo.GetCultureInfo("en-us"),
				TargetCulture = CultureInfo.GetCultureInfo("fr")
			};

			var xliff = new Xliff
			{
				File = file
			};

			xliff.AddSourceText(null);
		}

		/// <summary>
		/// Ensures if a null object is passed in as a segment, an exception is thrown.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void AddTranslation_NullTargetSegment_ThrowsException()
		{
			var file = new File
			{
				SourceCulture = CultureInfo.GetCultureInfo("en-us"),
				TargetCulture = CultureInfo.GetCultureInfo("fr")
			};

			var xliff = new Xliff
			{
				File = file
			};

			xliff.AddTranslation(new Segment(xliff.File.SourceCulture), null, "MTEdge");
		}
	}
}