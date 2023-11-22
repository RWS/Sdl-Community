using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using Multilingual.XML.FileType.Services;
using Multilingual.XML.FileType.Services.Entities;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.FileType.Preview
{
	public class InternalPreviewFileTweaker : AbstractFilePostTweaker, INativeOutputSettingsAware
	{
		private readonly XmlReaderFactory _xmlReaderFactory;

		private readonly AlternativeInputFileGenerator _alternativeInputFileGenerator;

		private readonly EntityMarkerConversionService _entityMarkerConversionService;
		public InternalPreviewFileTweaker(XmlReaderFactory xmlReaderFactory, AlternativeInputFileGenerator alternativeInputFileGenerator,
			EntityMarkerConversionService entityMarkerConversionService)
		{
			_xmlReaderFactory = xmlReaderFactory;
			_alternativeInputFileGenerator = alternativeInputFileGenerator;
			_entityMarkerConversionService = entityMarkerConversionService;
			RequireValidEncoding = false;
		}

		public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
			var newFileName = fileProperties.OriginalFilePath;

			var previewSuffix = "_htmlPreview.html";

			// set new extension
			if (!newFileName.EndsWith(previewSuffix, StringComparison.InvariantCultureIgnoreCase))
			{
				newFileName = newFileName + previewSuffix;
			}
			// strip the path
			newFileName = Path.GetFileName(newFileName);
			proposedFileInfo.Filename = newFileName;
		}

		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
		}

		public override void TweakFilePostWriting(INativeOutputFileProperties outputFileProperties)
		{
			var embeddedResource = typeof(PluginResources).Namespace + ".Resources.defaults.xsl";
			var xslTemplateFile = Path.GetTempFileName();
			WriteEmbeddedResourceToFile(embeddedResource, xslTemplateFile);

			var tempSdlXliffFile = _alternativeInputFileGenerator.GenerateTempFileWithHiddenEntities(outputFileProperties.OutputFilePath, outputFileProperties.Encoding.Encoding);

			var sb = new StringBuilder();

			using (var xr = _xmlReaderFactory.CreateReader(xslTemplateFile, true))
			{
				var xct = new XslCompiledTransform();

				xct.Load(xr);
				using (var xw = XmlWriter.Create(sb))
				{
					xct.Transform(tempSdlXliffFile, xw);
				}
			}
			
			File.Delete(xslTemplateFile);
			File.Delete(tempSdlXliffFile);

			using (var writer = new StreamWriter(outputFileProperties.OutputFilePath, false, outputFileProperties.Encoding.Encoding))
			{
				writer.Write(_entityMarkerConversionService.BackwardEntityMarkersConversion(sb.ToString()));
				writer.Flush();
			}

			string content;
			using (var reader = new StreamReader(outputFileProperties.OutputFilePath,
				outputFileProperties.Encoding.Encoding))
			{
				content = reader.ReadToEnd();
			}

			var regexSegment = new Regex(@"(?<fspan>(|\<SPAN\s+[^\>]+>\s*))&lt;" + Constants.MultilingualSegment + @"\s+pid=""(?<pid>[^""]+)""\s+sid=""(?<sid>[^""]+)""&gt;(?<content>(|.*?))&lt;/" + Constants.MultilingualSegment + @"&gt;(?<espan>(\s*</SPAN>|))", RegexOptions.Singleline);
			content = regexSegment.Replace(content, ReplaceSegment);

			var regexParagraphUnit = new Regex(@"(?<fspan>(|\<SPAN\s+[^\>]+>\s*))&lt;" + Constants.MultilingualParagraphUnit + @"\s+pid=""(?<pid>[^""]+)""&gt;(?<content>(|.*?))&lt;/" + Constants.MultilingualParagraphUnit + @"&gt;(?<espan>(\s*</SPAN>|))", RegexOptions.Singleline);
			content = regexParagraphUnit.Replace(content, ReplaceParagraphUnit);

			using (var writer = new StreamWriter(outputFileProperties.OutputFilePath, false, outputFileProperties.Encoding.Encoding))
			{
				writer.Write(content);
				writer.Flush();
			}

			//File.Delete(outputPath);


			base.TweakFilePostWriting(outputFileProperties);
		}

		private bool WriteEmbeddedResourceToFile(string embeddedResource, string outputFilePath)
		{

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource))
			{
				if (stream == null)
				{
					return false;
				}

				using (var reader = new BinaryReader(stream))
				{
					using (Stream writer = File.Create(outputFilePath))
					{
						var buffer = new byte[2048];
						while (true)
						{
							var current = reader.Read(buffer, 0, buffer.Length);
							if (current == 0)
							{
								break;
							}

							writer.Write(buffer, 0, current);
						}
					}
				}
			}

			return true;
		}

		private string ReplaceSegment(Match match)
		{
			var fspan = match.Groups["fspan"].Value;
			var paragraphId = match.Groups["pid"].Value;
			var segmentId = match.Groups["sid"].Value;
			var content = match.Groups["content"].Value;
			var espan = match.Groups["espan"].Value;

			var result = string.Format(@"<em id=""{0}"" class=""normal"" style=""font-style:normal"" onClick=""window.external.SelectSegment('{1}','{2}')"">{3}{4}{5}</em>"
				, segmentId, paragraphId, segmentId, fspan, content, espan);

			return result;
		}

		private string ReplaceParagraphUnit(Match match)
		{
			var fspan = match.Groups["fspan"].Value;
			var paragraphId = match.Groups["paragraph_id"].Value;
			var content = match.Groups["content"].Value;
			var espan = match.Groups["espan"].Value;

			var result = string.Format(@"<em id=""{0}"" class=""normal"" style=""font-style:normal"" onClick=""window.external.SelectParagraph('{1}')"">{2}{3}{4}</em>"
				, paragraphId, paragraphId, fspan, content, espan);

			return result;
		}
	}
}
