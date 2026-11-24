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
			var output = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlf");
			File.Copy(InBin("Fixtures", "sample.xlf"), output);
			try
			{
				new XsltPreviewTweaker(InBin("preview.xslt"))
					.TweakFilePostWriting(new NativeOutputFileProperties { OutputFilePath = output });

				var html = File.ReadAllText(output);
				Assert.Contains("<h1>XSLT preview</h1>", html);
				Assert.Contains("<td class=\"tgt\">Hallo Welt</td>", html);
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
