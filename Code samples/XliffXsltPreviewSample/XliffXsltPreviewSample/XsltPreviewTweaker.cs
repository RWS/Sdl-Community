using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
			string html;
			using (var output = new MemoryStream())
			{
				xslt.Transform(new XPathDocument(outputFileProperties.OutputFilePath), null, output);
				output.Position = 0;
				html = new StreamReader(output).ReadToEnd();
			}
			File.WriteAllText(outputFileProperties.OutputFilePath, AddSegmentSync(html), new UTF8Encoding(true));
		}

		private string AddSegmentSync(string html)
		{
			html = Regex.Replace(html, Regex.Escape(SegmentMarker.Open) + "(.+?)" + Regex.Escape(SegmentMarker.Close), "<span class=\"sdl-seg\" data-seg=\"$1\"></span>");
			string script;
			using (var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("XliffXsltPreviewSample.sync.js")))
			{
				script = "<script>" + reader.ReadToEnd() + "</script>";
			}
			var bodyEnd = html.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);
			return bodyEnd >= 0 ? html.Insert(bodyEnd, script) : html + script;
		}
	}
}
