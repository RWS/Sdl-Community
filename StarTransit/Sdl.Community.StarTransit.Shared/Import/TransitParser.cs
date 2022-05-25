using System;
using System.IO;
using System.Text;
using System.Xml;
using NLog;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TransitParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware, ISettingsAware
	{
		private IFileProperties _fileProperties;
		private readonly IFileService _fileService = new FileService();
		private XmlDocument _trgDocument;
		private XmlDocument _srcDocument;
		public event EventHandler<ProgressEventArgs> Progress;
		private int _totalTagCount;
		private int _tmpTotalTagCount;
		private int _srcSegmentTagCount;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public void SetFileProperties(IFileProperties properties)
		{
			_fileProperties = properties;
		}

		public void StartOfInput()
		{
			OnProgress(0);
			var trgFilePath = _fileProperties?.FileConversionProperties?.OriginalFilePath;
			if (!File.Exists(trgFilePath)) return;

			_trgDocument = new XmlDocument {PreserveWhitespace = true};
			_srcDocument = new XmlDocument {PreserveWhitespace = true};
			_trgDocument.Load(trgFilePath);

			var directoryPath = Path.GetDirectoryName(trgFilePath);
			if (string.IsNullOrEmpty(directoryPath)) return;

			//search for source tm file corresponding to the target tm
			foreach (var file in Directory.GetFiles(directoryPath))
			{
				var currentFileName = Path.GetFileNameWithoutExtension(file);
				var targetFileName = Path.GetFileNameWithoutExtension(trgFilePath);
				var currentFileExtension = Path.GetExtension(file);
				var targetFileExtension = Path.GetExtension(trgFilePath);
				if (currentFileName == targetFileName && currentFileExtension != targetFileExtension)
				{
					_srcDocument.Load(file);
					break;
				}
			}
		}

		public void EndOfInput()
		{
			OnProgress(100);
			_trgDocument = null;
		}

		protected virtual void OnProgress(byte percent)
		{
			Progress?.Invoke(this, new ProgressEventArgs(percent));
		}

		public IDocumentProperties DocumentProperties { get; set; }

		public IBilingualContentHandler Output
		{
			get;
			set;
		}

		public bool ParseNext()
		{
			try
			{
				if (DocumentProperties == null)
				{
					DocumentProperties = ItemFactory.CreateDocumentProperties();
				}
				Output.Initialize(DocumentProperties);

				var fileInfo = ItemFactory.CreateFileProperties();
				fileInfo.FileConversionProperties = _fileProperties.FileConversionProperties;
				Output.SetFileProperties(fileInfo);

				var segNodes = _trgDocument?.SelectNodes("//Seg");
				if (segNodes != null)
				{
					var totalUnitCount = segNodes.Count;
					var currentUnitCount = 0;

					foreach (XmlNode item in segNodes)
					{
						var dataAttribute = item.SelectSingleNode("./@Data")?.InnerText;
						if (dataAttribute != null)
						{
							var dataAttributeHexCode = _fileService.ConvertStringToHex(dataAttribute, Encoding.Unicode);
							if (_fileService.IsValidNode(dataAttributeHexCode))
							{
								Output.ProcessParagraphUnit(CreateParagraphUnit(item, dataAttributeHexCode));
							}
						}
						// update the progress report   
						currentUnitCount++;
						OnProgress(Convert.ToByte(Math.Round(100 * ((decimal)currentUnitCount / totalUnitCount), 0)));
					}
				}

				Output.FileComplete();
				Output.Complete();
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
			return false;
		}

		// helper function for creating paragraph units
		private IParagraphUnit CreateParagraphUnit(XmlNode xmlUnit,string dataAttributeHexCode)
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
				segmentPairProperties.ConfirmationLevel = CreateConfirmationLevel(dataAttributeHexCode);
				tuOrg.MatchPercent = CreateMatchValue(item);

				// add source segment to paragraph unit
				var srcSegment = CreateSegment(_srcDocument.SelectSingleNode("//Seg[@SegID='" + id + "']"),
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

		private ConfirmationLevel CreateConfirmationLevel(string dataAttributeHexCode)
		{
			var sdlxliffLevel = ConfirmationLevel.Translated;

			var statusCode = dataAttributeHexCode.Substring(0, 2).ToLower();

			if (statusCode == "02")
				sdlxliffLevel = ConfirmationLevel.Unspecified;
			if (statusCode == "0e")
				sdlxliffLevel = ConfirmationLevel.ApprovedSignOff;
			if (statusCode == "0f")
				sdlxliffLevel = ConfirmationLevel.ApprovedSignOff;
			if (statusCode == "0a")
				sdlxliffLevel = ConfirmationLevel.Translated;
			if (statusCode == "06")
				sdlxliffLevel = ConfirmationLevel.Translated;
			if (statusCode == "04")
				sdlxliffLevel = ConfirmationLevel.Translated;
			if (statusCode == "0c")
				sdlxliffLevel = ConfirmationLevel.ApprovedTranslation;
			if (statusCode == "08")
				sdlxliffLevel = ConfirmationLevel.Draft;

			return sdlxliffLevel;
		}

		private byte CreateMatchValue(XmlNode segmentXml)
		{
			byte matchValue = 0;
			if (segmentXml is null) return matchValue;
			var data = segmentXml.SelectSingleNode("./@Data").InnerText;

			if (data.Contains("\\") && segmentXml.SelectSingleNode(".").InnerText != "")
			{
				matchValue = 100;
			}

			return matchValue;
		}

		// helper function for creating segment objects
		private ISegment CreateSegment(XmlNode segNode, ISegmentPairProperties pair, bool source)
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

		public void InitializeSettings(Sdl.Core.Settings.ISettingsBundle settingsBundle, string configurationId)
		{
			//loading of filter settings
		}

		public void Dispose()
		{
			_trgDocument = null;
		}
	}
}