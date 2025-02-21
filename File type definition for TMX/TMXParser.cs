using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.FileType.TMX
{
	class TMXParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware, ISettingsAware
	{
		private const string START_TAG = "bpt";
		private const string END_TAG = "ept";
		
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private XmlDocument _document;
		public event EventHandler<ProgressEventArgs> Progress;
		int segId;




		public void SetFileProperties(IFileProperties properties)
		{
			_fileProperties = properties;
		}


		public void StartOfInput()
		{
			OnProgress(0);

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.XmlResolver = null;
			settings.DtdProcessing = DtdProcessing.Ignore;
			XmlReader xmlReader = XmlTextReader.Create(_fileProperties.FileConversionProperties.OriginalFilePath, settings);

			_document = new XmlDocument();
			_document.PreserveWhitespace = true;
			_document.Load(xmlReader);
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
			int totalUnitCount = _document.SelectNodes("//tu").Count;
			int currentUnitCount = 0;
			foreach (XmlNode item in _document.SelectNodes("//tu"))
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

			segId++;
			string id = Convert.ToString(segId);
			paragraphUnit.Properties.Contexts = CreateContext("Paragraph", id);



			// create segment pair object
			ISegmentPairProperties segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
			ITranslationOrigin tuOrg = ItemFactory.CreateTranslationOrigin();

			// assign the appropriate confirmation level to the segment pair            
			segmentPairProperties.ConfirmationLevel = CreateConfirmationLevel(xmlUnit);
			tuOrg.MatchPercent = this.CreateMatchValue();


			// add source segment to paragraph unit
			ISegment srcSegment = CreateSegment(xmlUnit.SelectSingleNode("tuv[1]/seg"), segmentPairProperties);
			paragraphUnit.Source.Add(srcSegment);

			// add target segment to paragraph unit
			ISegment trgSegment = CreateSegment(xmlUnit.SelectSingleNode("tuv[2]/seg"), segmentPairProperties);
			paragraphUnit.Target.Add(trgSegment);


			if (tuOrg.MatchPercent > 0)
				tuOrg.OriginType = DefaultTranslationOrigin.TranslationMemory;

			segmentPairProperties.TranslationOrigin = tuOrg;


			return paragraphUnit;
		}

		private ConfirmationLevel CreateConfirmationLevel(XmlNode segmentXml)
		{
			ConfirmationLevel sdlxliffLevel = ConfirmationLevel.Translated;

			string confirmationValue = "Translated";

			if (segmentXml.SelectSingleNode("prop[@type='x-ConfirmationLevel']") != null)
			{
				confirmationValue = segmentXml.SelectSingleNode("prop[@type='x-ConfirmationLevel']").InnerText;
			}


			switch (confirmationValue)
			{
				case "Translated":
					sdlxliffLevel = ConfirmationLevel.Translated;
					break;
				case "Draft":
					sdlxliffLevel = ConfirmationLevel.Draft;
					break;
				case "ApprovedTranslation":
					sdlxliffLevel = ConfirmationLevel.ApprovedTranslation;
					break;
				case "ApprovedSignOff":
					sdlxliffLevel = ConfirmationLevel.ApprovedSignOff;
					break;
				case "RejectedTranslation":
					sdlxliffLevel = ConfirmationLevel.RejectedTranslation;
					break;
				case "RejectedSignOff":
					sdlxliffLevel = ConfirmationLevel.RejectedSignOff;
					break;
				default:
					sdlxliffLevel = ConfirmationLevel.Translated;
					break;
			}




			return sdlxliffLevel;
		}

		private Byte CreateMatchValue()
		{
			Byte matchValue;

			matchValue = 100;

			return matchValue;
		}

		// if not found, returns ""
		private static string GetAttribute(XmlNode node, string attributeName)
		{
			var found = node.Attributes?.OfType<XmlAttribute>().FirstOrDefault(a => a.Name.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
			var value = found?.Value;
			return value ?? "";
		}

		// helper function for creating segment objects
		private ISegment CreateSegment(XmlNode segNode, ISegmentPairProperties pair)
		{
			ISegment segment = ItemFactory.CreateSegment(pair);
			Stack<IAbstractMarkupDataContainer> segmentStack = new Stack<IAbstractMarkupDataContainer>();
			segmentStack.Push(segment);

			foreach (XmlNode item in segNode.ChildNodes)
			{
				IAbstractMarkupDataContainer parent = segmentStack.Peek();

				if (item.NodeType == XmlNodeType.Text || item.NodeType == XmlNodeType.Whitespace)
				{
					parent.Add(CreateText(item.InnerText));
				}

				if (item.NodeType == XmlNodeType.Element)
				{
					switch (item.Name)
					{
						case START_TAG:
							ITagPair tagPair = CreateTagPair(item);
							parent.Add(tagPair);
							segmentStack.Push(tagPair);
							break;

						case END_TAG:
                            UpdateEndTag(parent as ITagPair, item);
							segmentStack.Pop();
							break;
						default:
							parent.Add(CreatePhTag(item));
							break;
					}

				}
			}
			return segment;
		}

        private void UpdateEndTag(ITagPair tagPair, XmlNode item)
        {
			tagPair.EndTagProperties.TagContent = item.OuterXml;
			tagPair.EndTagProperties.DisplayText = item.Name;
			tagPair.EndTagProperties.CanHide = false;
		}

        private IText CreateText(string segText)
		{
			ITextProperties textProperties = PropertiesFactory.CreateTextProperties(segText);
			IText textContent = ItemFactory.CreateText(textProperties);

			return textContent;
		}

        private ITagPair CreateTagPair(XmlNode item)
        {
			var startTagProperties = PropertiesFactory.CreateStartTagProperties(item.OuterXml);
			startTagProperties.DisplayText = item.Name;
			startTagProperties.CanHide = false;

			return ItemFactory.CreateTagPair(startTagProperties, PropertiesFactory.CreateEndTagProperties(END_TAG));
		}

		private IPlaceholderTag CreatePhTag(XmlNode item)
		{

			var phTagProperties = PropertiesFactory.CreatePlaceholderTagProperties(item.Name);

			phTagProperties.TagContent = item.OuterXml;
			phTagProperties.DisplayText = item.Name;
			phTagProperties.CanHide = false;

			IPlaceholderTag phTag = ItemFactory.CreatePlaceholderTag(phTagProperties);
			return phTag;
		}

		private IContextProperties CreateContext(string spec, string unitID)
		{
			IContextProperties contextProperties = PropertiesFactory.CreateContextProperties();
			IContextInfo contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.TranslatableContent);
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