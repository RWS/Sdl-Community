using System.IO;
using System.Xml.XPath;
using System.Xml.Xsl;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace XliffXsltPreviewSample
{
	public class XsltPreviewTweaker : IFilePostTweaker
	{
		private readonly string _xsltPath;

		public XsltPreviewTweaker(string xsltPath)
		{
			_xsltPath = xsltPath;
		}

		public INativeTextLocationMessageReporter MessageReporter { get; set; }

		public bool Enabled { get; set; } = true;

		public void TweakFilePostWriting(INativeOutputFileProperties outputFileProperties)
		{
			var xslt = new XslCompiledTransform();
			xslt.Load(_xsltPath);
			using (var html = new MemoryStream())
			{
				xslt.Transform(new XPathDocument(outputFileProperties.OutputFilePath), null, html);
				File.WriteAllBytes(outputFileProperties.OutputFilePath, html.ToArray());
			}
		}
	}
}
