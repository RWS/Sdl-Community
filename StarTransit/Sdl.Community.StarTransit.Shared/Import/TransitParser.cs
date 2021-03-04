using System;
using System.IO;
using System.Text;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Utils;
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
		private IDocumentProperties _documentProperties;
		private XmlDocument _document;
		private XmlDocument _srcDocument;
		public event EventHandler<ProgressEventArgs> Progress;

		private int _totalTagCount;
		private int _tmpTotalTagCount;
		private int _srcSegmentTagCount;

		public void SetFileProperties(IFileProperties properties)
		{
			_fileProperties = properties;
		}

		public void StartOfInput()
		{
			try
			{
				OnProgress(0);
				_document = new XmlDocument();
				_document.PreserveWhitespace = true;
				_srcDocument = new XmlDocument();
				_srcDocument.PreserveWhitespace = true;
				var trgFilePath = _fileProperties.FileConversionProperties.OriginalFilePath;
				_document.Load(trgFilePath);

				//TODO: Refactor
				var pos = trgFilePath.LastIndexOf("\\");
				var path = trgFilePath.Substring(0, pos);

				foreach (var fileName in Directory.GetFiles(path))
				{
					//TODO: Refactor code
					if (fileName.Substring(0, fileName.Length - 4) == trgFilePath.Substring(0, trgFilePath.Length - 4) &&
						fileName.Substring(fileName.Length - 4, 4) != trgFilePath.Substring(trgFilePath.Length - 4, 4))
					{
						_srcDocument.Load(fileName);
						//TODO: add break;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"StartOfInput method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		public void EndOfInput()
		{
			OnProgress(100);
			_document = null;
		}

		protected virtual void OnProgress(byte percent)
		{
			Progress?.Invoke(this, new ProgressEventArgs(percent));
		}

		public IDocumentProperties DocumentProperties
		{
			get
			{
				return _documentProperties;
			}
			set
			{
				_documentProperties = value;
			}
		}

		public IBilingualContentHandler Output
		{
			get;
			set;
		}

		public bool ParseNext()
		{
			try
			{
				if (_documentProperties == null)
				{
					_documentProperties = ItemFactory.CreateDocumentProperties();
				}
				Output.Initialize(_documentProperties);

				var fileInfo = ItemFactory.CreateFileProperties();
				fileInfo.FileConversionProperties = _fileProperties.FileConversionProperties;
				Output.SetFileProperties(fileInfo);

				// variables for the progress report
				if (!(_document is null))
				{
					//TODO: get the nodes in a variable
					var totalUnitCount = _document.SelectNodes("//Seg").Count;
					var currentUnitCount = 0;
					foreach (XmlNode item in _document.SelectNodes("//Seg"))
					{
						Output.ProcessParagraphUnit(CreateParagraphUnit(item));

						// update the progress report   
						currentUnitCount++;
						OnProgress(Convert.ToByte(Math.Round(100 * ((decimal) currentUnitCount / totalUnitCount), 0)));
					}
				}

				Output.FileComplete();
				Output.Complete();
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ParseNext method: {ex.Message}\n {ex.StackTrace}");
			}
			return false;
		}

		// helper function for creating paragraph units
		private IParagraphUnit CreateParagraphUnit(XmlNode xmlUnit)
		{
			try
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
					var srcSegment = CreateSegment(_srcDocument.SelectSingleNode("//Seg[@SegID='" + id + "']"), segmentPairProperties, true);
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
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateParagraphUnit method: {ex.Message}\n {ex.StackTrace}");
			}
			return null;
		}

		private ConfirmationLevel CreateConfirmationLevel(XmlNode segmentXml)
		{
			var sdlxliffLevel = ConfirmationLevel.Translated;
			try
			{
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
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateConfirmationLevel method: {ex.Message}\n {ex.StackTrace}");
			}
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

		// helper function for creating segment objects
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


		private IText CreateText(string segText)
		{
			var textProperties = PropertiesFactory.CreateTextProperties(segText);
			var textContent = ItemFactory.CreateText(textProperties);

			return textContent;
		}

		private IPlaceholderTag CreatePhTag(string tagContent, XmlNode item, bool source)
		{
			try
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
			catch (Exception ex)
			{
				Log.Logger.Error($"CreatePhTag method: {ex.Message}\n {ex.StackTrace}");
			}
			return null;
		}

		private IContextProperties CreateContext(string spec, string unitID)
		{
			try
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
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateContext method: {ex.Message}\n {ex.StackTrace}");
			}
			return null;
		}

		public void InitializeSettings(Sdl.Core.Settings.ISettingsBundle settingsBundle, string configurationId)
		{
			//loading of filter settings
		}

		public void Dispose()
		{
			_document = null;
		}
	}
}