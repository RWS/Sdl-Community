using System;
using System.IO;
using System.Text;
using System.Xml;
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

		private int totalTagCount;
		private int tmpTotalTagCount;
		private int srcSegmentTagCount;

		private ISegment _srcSegment;
		private ISegment _trgSegment;


		public void SetFileProperties(IFileProperties properties)
		{
			_fileProperties = properties;
		}


		public void StartOfInput()
		{
			OnProgress(0);
			_document = new XmlDocument();
			_document.PreserveWhitespace = true;
			_srcDocument = new XmlDocument();
			_srcDocument.PreserveWhitespace = true;
			string trgFilePath = _fileProperties.FileConversionProperties.OriginalFilePath;
			_document.Load(trgFilePath);

			int pos = trgFilePath.LastIndexOf("\\");
			string path = trgFilePath.Substring(0, pos);

			foreach (string fileName in Directory.GetFiles(path))
			{
				if (fileName.Substring(0, fileName.Length - 4) == trgFilePath.Substring(0, trgFilePath.Length - 4) &&
					fileName.Substring(fileName.Length - 4, 4) != trgFilePath.Substring(trgFilePath.Length - 4, 4))
				{
					_srcDocument.Load(fileName);

				}
			}
		}


		public void EndOfInput()
		{
			OnProgress(100);
			_document = null;
		}

		protected virtual void OnProgress(byte percent)
		{
			if (Progress != null)
			{
				Progress(this, new ProgressEventArgs(percent));
			}
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
			if (_documentProperties == null)
			{
				_documentProperties = ItemFactory.CreateDocumentProperties();
			}
			Output.Initialize(_documentProperties);

			IFileProperties fileInfo = ItemFactory.CreateFileProperties();
			fileInfo.FileConversionProperties = _fileProperties.FileConversionProperties;
			Output.SetFileProperties(fileInfo);

			// variables for the progress report
			int totalUnitCount = _document.SelectNodes("//Seg").Count;
			int currentUnitCount = 0;
			foreach (XmlNode item in _document.SelectNodes("//Seg"))
			{
				Output.ProcessParagraphUnit(CreateParagraphUnit(item));

				// update the progress report   
				currentUnitCount++;
				OnProgress(Convert.ToByte(Math.Round(100 * ((decimal)currentUnitCount / totalUnitCount), 0)));
			}

			Output.FileComplete();
			Output.Complete();

			return false;
		}

		// helper function for creating paragraph units
		private IParagraphUnit CreateParagraphUnit(XmlNode xmlUnit)
		{
			// create paragraph unit object
			IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

			// create paragraph unit context
			string id = xmlUnit.SelectSingleNode("./@SegID").InnerText;
			paragraphUnit.Properties.Contexts = CreateContext("Paragraph", id);


			foreach (XmlNode item in xmlUnit.SelectNodes("."))
			{
				// create segment pair object
				ISegmentPairProperties segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
				ITranslationOrigin tuOrg = ItemFactory.CreateTranslationOrigin();

				// assign the appropriate confirmation level to the segment pair            
				segmentPairProperties.ConfirmationLevel = CreateConfirmationLevel(item);
				tuOrg.MatchPercent = CreateMatchValue(item);

				// add source segment to paragraph unit
				ISegment srcSegment = CreateSegment(_srcDocument.SelectSingleNode("//Seg[@SegID='" + id + "']"), segmentPairProperties, true);
				paragraphUnit.Source.Add(srcSegment);
				// add target segment to paragraph unit if available
				if (item.SelectSingleNode(".") != null)
				{
					ISegment trgSegment = CreateSegment(item.SelectSingleNode("."), segmentPairProperties, false);
					paragraphUnit.Target.Add(trgSegment);
				}
				else
				{
					item.SelectSingleNode(".").InnerText = "";
					ISegment trgSegment = CreateSegment(item.SelectSingleNode("."), segmentPairProperties, false);
					paragraphUnit.Target.Add(trgSegment);
				}

				if (tuOrg.MatchPercent > 0)
					tuOrg.OriginType = DefaultTranslationOrigin.TranslationMemory;


				segmentPairProperties.TranslationOrigin = tuOrg;

			}

			return paragraphUnit;
		}

		private ConfirmationLevel CreateConfirmationLevel(XmlNode segmentXml)
		{

			string data = segmentXml.SelectSingleNode("./@Data").InnerText;

			ConfirmationLevel sdlxliffLevel = ConfirmationLevel.Translated;


			Byte[] stringBytes = Encoding.Unicode.GetBytes(data);
			StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
			foreach (byte b in stringBytes)
			{
				sbBytes.AppendFormat("{0:X2}", b);
			}

			string statusCode = sbBytes.ToString().Substring(0, 2).ToLower();

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


		private byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];

			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}


		private Byte CreateMatchValue(XmlNode segmentXml)
		{
			string data = segmentXml.SelectSingleNode("./@Data").InnerText;
			Byte matchValue = 0;

			if (data.Contains("\\") && segmentXml.SelectSingleNode(".").InnerText != "")
			{
				matchValue = 100;
			}

			return matchValue;
		}

		// helper function for creating segment objects
		private ISegment CreateSegment(XmlNode segNode, ISegmentPairProperties pair, bool source)
		{
			ISegment segment = ItemFactory.CreateSegment(pair);

			if (source)
			{
				_srcSegment = segment;


				srcSegmentTagCount = 0;
				if (totalTagCount < tmpTotalTagCount)
				{
					totalTagCount = tmpTotalTagCount;
				}
			}
			else
			{
				_trgSegment = segment;
				totalTagCount = totalTagCount - srcSegmentTagCount;
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
			ITextProperties textProperties = PropertiesFactory.CreateTextProperties(segText);
			IText textContent = ItemFactory.CreateText(textProperties);

			return textContent;
		}

		private IPlaceholderTag CreatePhTag(string tagContent, XmlNode item, bool source)
		{
			IPlaceholderTagProperties phTagProperties = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
			IPlaceholderTag phTag = ItemFactory.CreatePlaceholderTag(phTagProperties);

			string cont;
			if (item.NextSibling == null)
				cont = "";
			else
				cont = item.NextSibling.Value;

			phTagProperties.SegmentationHint = SegmentationHint.IncludeWithText;
			phTagProperties.TagContent = item.OuterXml;
			phTagProperties.DisplayText = item.Name;
			phTagProperties.CanHide = false;


			//determine tag id
			if (source)
			{
				/*Sdl.FileTypeSupport.Framework.NativeApi.TagId thisId = 
                    new Sdl.FileTypeSupport.Framework.NativeApi.TagId(totalTagCount.ToString());

                phTagProperties.TagId = thisId;*/
				totalTagCount += 1;
				tmpTotalTagCount += 1;
				srcSegmentTagCount += 1;
			}

			return phTag;
		}

		private IContextProperties CreateContext(string spec, string unitID)
		{
			IContextProperties contextProperties = PropertiesFactory.CreateContextProperties();
			IContextInfo contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
			contextInfo.Purpose = ContextPurpose.Information;

			// add unit id as metadata
			IContextInfo contextId = PropertiesFactory.CreateContextInfo("UnitId");
			contextId.SetMetaData("UnitID", unitID);
			contextId.Description = "Original paragraph unit id";
			contextId.DisplayCode = "ID";

			contextProperties.Contexts.Add(contextInfo);
			contextProperties.Contexts.Add(contextId);

			return contextProperties;
		}



		private ICommentProperties CreateComment(XmlNode item)
		{
			ICommentProperties commentProperties = PropertiesFactory.CreateCommentProperties();

			return commentProperties;
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