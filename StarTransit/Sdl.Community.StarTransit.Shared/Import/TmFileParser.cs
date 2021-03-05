using System;
using System.IO;
using System.Text;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TmFileParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware,ISettingsAware 
	{
		private XmlDocument _sourceTmFile;
		private XmlDocument _targetTmFile;
		private IFileProperties _fileProperties;
		private int _totalTagCount;
		private int _tmpTotalTagCount;
		private int _srcSegmentTagCount;
		private IFileService _fileService = new FileService();

		public void StartOfInput()
		{
			OnProgress(0);
			_sourceTmFile = new XmlDocument { PreserveWhitespace = true };
			var targetTmFilePath = _fileProperties?.FileConversionProperties?.OriginalFilePath;

			if (!File.Exists(targetTmFilePath)) return;
			_targetTmFile = new XmlDocument {PreserveWhitespace = true};
			_targetTmFile.Load(targetTmFilePath);

			var directoryPath = Path.GetDirectoryName(targetTmFilePath);
			if (string.IsNullOrEmpty(directoryPath)) return;

			//search for source tm file corresponding to the target tm
			foreach (var file in Directory.GetFiles(directoryPath))
			{
				var currentFileName = Path.GetFileNameWithoutExtension(file);
				var targetFileName = Path.GetFileNameWithoutExtension(targetTmFilePath);
				var currentFileExtension = Path.GetExtension(file);
				var targetFileExtension = Path.GetExtension(targetTmFilePath);
				if (currentFileName == targetFileName && currentFileExtension != targetFileExtension)
				{
					_sourceTmFile.Load(file);
					break;
				}
			}
		}

		public void Dispose()
		{

		}

		public bool ParseNext()
		{
			if (DocumentProperties is null)
			{
				DocumentProperties = ItemFactory.CreateDocumentProperties();
			}

			Output.Initialize(DocumentProperties);

			var fileInfo = ItemFactory.CreateFileProperties();
			fileInfo.FileConversionProperties = _fileProperties.FileConversionProperties;
			Output.SetFileProperties(fileInfo);
			var segNodes = _targetTmFile?.SelectNodes("//Seg");
			if (segNodes != null)
			{
				var totalUnitCount = segNodes.Count;
				var currentUnitCount = 0;
				foreach (XmlNode item in segNodes)
				{
					if (_fileService.IsValidNode(item))
					{
						Output.ProcessParagraphUnit(CreateParagraphUnit(item));
					}

					// update the progress report   
					currentUnitCount++;
					OnProgress(Convert.ToByte(Math.Round(100 * ((decimal) currentUnitCount / totalUnitCount), 0)));
				}
			}
			Output.FileComplete();
			Output.Complete();
			return false;
		}

		public event EventHandler<ProgressEventArgs> Progress;
		public IDocumentProperties DocumentProperties { get; set; }
		public IBilingualContentHandler Output { get; set; }

		public void SetFileProperties(IFileProperties properties)
		{
			_fileProperties = properties;
		}

		protected virtual void OnProgress(byte percent)
		{
			Progress?.Invoke(this, new ProgressEventArgs(percent));
		}

		public void EndOfInput()
		{
			OnProgress(100);
			_sourceTmFile = null;
			_targetTmFile = null;
		}

		public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
		{

		}

		private IParagraphUnit CreateParagraphUnit(XmlNode xmlUnit)
		{
			// create paragraph unit object
			var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

			// create paragraph unit context
			var id = xmlUnit.SelectSingleNode("./@SegID").InnerText;
			paragraphUnit.Properties.Contexts = CreateContext("Paragraph", id);

			foreach (XmlNode item in xmlUnit.SelectNodes("."))
			{
				// create segment pair object
				var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
				var tuOrg = ItemFactory.CreateTranslationOrigin();

				// assign the appropriate confirmation level to the segment pair            
				segmentPairProperties.ConfirmationLevel = CreateConfirmationLevel(item);
				tuOrg.MatchPercent = CreateMatchValue(item);

				// add source segment to paragraph unit
				var srcSegment = CreateSegment(_sourceTmFile.SelectSingleNode("//Seg[@SegID='" + id + "']"),
					segmentPairProperties, true);
				paragraphUnit.Source.Add(srcSegment);
				// add target segment to paragraph unit if available
				if (item.SelectSingleNode(".") != null)
				{
					var trgSegment = CreateSegment(item.SelectSingleNode("."), segmentPairProperties, false);
					paragraphUnit.Target.Add(trgSegment);
				}
				else
				{
					item.SelectSingleNode(".").InnerText = "";
					var trgSegment = CreateSegment(item.SelectSingleNode("."), segmentPairProperties, false);
					paragraphUnit.Target.Add(trgSegment);
				}

				if (tuOrg.MatchPercent > 0)
				{
					tuOrg.OriginType = DefaultTranslationOrigin.TranslationMemory;
				}

				segmentPairProperties.TranslationOrigin = tuOrg;
				return paragraphUnit;
			}

			return null;
		}
		
		private IContextProperties CreateContext(string spec, string unitID)
		{
			var contextProperties = PropertiesFactory.CreateContextProperties();
			var contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
			contextInfo.Purpose = ContextPurpose.Information;

			// add unit id as metadata
			var contextId = PropertiesFactory.CreateContextInfo("UnitId");
			contextId.SetMetaData("UnitID", unitID);
			contextId.Description = "Original paragraph unit id";
			contextId.DisplayCode = "ID";

			contextProperties.Contexts.Add(contextInfo);
			contextProperties.Contexts.Add(contextId);

			return contextProperties;
		}
		private ISegment CreateSegment(XmlNode segNode, ISegmentPairProperties pair, bool source)
		{
			try
			{
				var segment = ItemFactory.CreateSegment(pair);
				if (source)
				{
					_srcSegmentTagCount = 0;
					if (_totalTagCount < _tmpTotalTagCount)
					{
						_totalTagCount = _tmpTotalTagCount;
					}
				}
				else
				{
					_totalTagCount = _totalTagCount - _srcSegmentTagCount;
				}

				if (segNode != null)
				{
					foreach (XmlNode item in segNode.ChildNodes)
					{
						if (item.NodeType == XmlNodeType.Text)
						{
							segment.Add(CreateText(item.InnerText));
						}

						if (item.NodeType == XmlNodeType.Element)
						{
							segment.Add(CreatePhTag(item.Name, item, source));
						}

						if (item.NodeType == XmlNodeType.Whitespace)
						{
							segment.Add(CreateText(" "));
						}
					}
				}

				return segment;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateSegment method: {ex.Message}\n {ex.StackTrace}");
			}

			return null;
		}

		private ConfirmationLevel CreateConfirmationLevel(XmlNode segmentXml)
		{
			var sdlxliffLevel = ConfirmationLevel.Translated;

			var data = segmentXml.SelectSingleNode("./@Data").InnerText;
			var stringBytes = Encoding.Unicode.GetBytes(data);
			var sbBytes = new StringBuilder(stringBytes.Length * 2);
			foreach (var b in stringBytes)
			{
				sbBytes.AppendFormat("{0:X2}", b);
			}

			var statusCode = sbBytes.ToString().Substring(0, 2).ToLower();

			if (statusCode == "02")
				sdlxliffLevel = ConfirmationLevel.Unspecified;
			if (statusCode == "0e")
				sdlxliffLevel = ConfirmationLevel.ApprovedTranslation;
			if (statusCode == "0f")
				sdlxliffLevel = ConfirmationLevel.ApprovedSignOff;
			if (statusCode == "0a")
				sdlxliffLevel = ConfirmationLevel.Translated;
			if (statusCode == "0c")
				sdlxliffLevel = ConfirmationLevel.Translated;
			if (statusCode == "08")
				sdlxliffLevel = ConfirmationLevel.Translated;

			return sdlxliffLevel;
		}

		private byte CreateMatchValue(XmlNode segmentXml)
		{
			byte matchValue = 0;
			if (!(segmentXml is null))
			{
				var data = segmentXml.SelectSingleNode("./@Data").InnerText;

				if (data.Contains("\\") && segmentXml.SelectSingleNode(".").InnerText != "")
				{
					matchValue = 100;
				}
			}

			return matchValue;
		}

		private IText CreateText(string segText)
		{
			var textProperties = PropertiesFactory.CreateTextProperties(segText);
			var textContent = ItemFactory.CreateText(textProperties);

			return textContent;
		}

		private IPlaceholderTag CreatePhTag(string tagContent, XmlNode item, bool source)
		{
			var phTagProperties = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
			var phTag = ItemFactory.CreatePlaceholderTag(phTagProperties);

			var cont = item.NextSibling != null ? item.NextSibling.Value : string.Empty;

			phTagProperties.SegmentationHint = SegmentationHint.IncludeWithText;
			phTagProperties.TagContent = item.OuterXml;
			phTagProperties.DisplayText = item.Name;
			phTagProperties.CanHide = false;

			//determine tag id
			if (source)
			{
				_totalTagCount += 1;
				_tmpTotalTagCount += 1;
				_srcSegmentTagCount += 1;
			}

			return phTag;
		}
	}
}
