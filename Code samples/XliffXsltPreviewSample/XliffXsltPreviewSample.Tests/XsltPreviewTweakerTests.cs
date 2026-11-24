using System;
using System.IO;
using Sdl.FileTypeSupport.Framework.Native;
using Xunit;

namespace XliffXsltPreviewSample.Tests
{
	public class XsltPreviewTweakerTests
	{
		[Fact]
		public void TweakFilePostWriting_ReplacesGeneratedXliffWithXsltHtml()
		{
			var html = Transform(xliff => xliff);

			Assert.Contains("<h1>XSLT preview</h1>", html);
			Assert.Contains("<td class=\"tgt\">Hallo Welt</td>", html);
		}

		[Fact]
		public void TweakFilePostWriting_TurnsSegmentMarkersIntoAnchors()
		{
			var html = Transform(xliff => xliff.Replace("<target>Hallo Welt", "<target>{{SDLSEG:pu1DELIMITER1}}Hallo Welt"));

			Assert.Contains("<td class=\"tgt\"><span class=\"sdl-seg\" data-seg=\"pu1DELIMITER1\"></span>Hallo Welt</td>", html);
			Assert.DoesNotContain("{{SDLSEG:", html);
		}

		[Fact]
		public void TweakFilePostWriting_AddsStudioSyncScriptBeforeBodyEnd()
		{
			var html = Transform(xliff => xliff);

			var script = html.IndexOf("function onSegmentSelectedInEditor(", StringComparison.Ordinal);
			Assert.True(script >= 0, "sync script missing");
			Assert.True(script < html.IndexOf("</body>", StringComparison.Ordinal), "sync script not inside body");
			Assert.Contains("applyInitialSelection", html);
		}

		private static string Transform(Func<string, string> editFixture)
		{
			var output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlf");
			File.WriteAllText(output, editFixture(File.ReadAllText(InBin("Fixtures", "sample.xlf"))));
			try
			{
				new XsltPreviewTweaker(InBin("preview.xslt"))
					.TweakFilePostWriting(new NativeOutputFileProperties { OutputFilePath = output });
				return File.ReadAllText(output);
			}
			finally
			{
				File.Delete(output);
			}
		}

		private static string InBin(params string[] parts) =>
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine(parts));
	}
}
