using System;
using System.ComponentModel;
using System.Xml;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
    /// <summary>
    /// The template to apply to the project
    /// </summary>
    public class ApplyTemplate
    {
        /// <summary>
        /// The template name
        /// </summary>
        private string _templateName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTemplate"/> class.
        /// </summary>
        public ApplyTemplate() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTemplate"/> class.
        /// </summary>
        /// <param name="sourceXml">The source XML.</param>
        public ApplyTemplate(XmlNode sourceXml)
        {
	        var selectSingleNode = sourceXml.SelectSingleNode("@name");
	        if (selectSingleNode != null) Name = selectSingleNode.Value;
	        var singleNode = sourceXml.SelectSingleNode("@location");
	        if (singleNode != null)
		        FileLocation = singleNode.Value;
	        Uri = null;
	        var xmlNode = sourceXml.SelectSingleNode("@id");
	        if (xmlNode != null) Id = new Guid(xmlNode.Value);
	        TranslationProvidersAllLanguages = GetApplyTemplateOptions(sourceXml, "tpal");
            TranslationProvidersSpecificLanguages = GetApplyTemplateOptions(sourceXml, "tpsl");
            TranslationMemoriesAllLanguages = GetApplyTemplateOptions(sourceXml, "tmal");
            TranslationMemoriesSpecificLanguages = GetApplyTemplateOptions(sourceXml, "tmsl");
            TerminologyTermbases = GetApplyTemplateOptions(sourceXml, "tbtb");
            TerminologySearchSettings = GetApplyTemplateOptions(sourceXml, "tbss");
            TranslationQualityAssessment = GetApplyTemplateOptions(sourceXml, "tqa");
            VerificationQaChecker30 = GetApplyTemplateOptions(sourceXml, "qaqa");
            VerificationTagVerifier = GetApplyTemplateOptions(sourceXml, "qatg");
            VerificationTerminologyVerifier = GetApplyTemplateOptions(sourceXml, "qatv");
            VerificationNumberVerifier = GetApplyTemplateOptions(sourceXml, "qanv");
            VerificationGrammarChecker = GetApplyTemplateOptions(sourceXml, "qagc");
            BatchTasksAllLanguages = GetApplyTemplateOptions(sourceXml, "btal");
            BatchTasksSpecificLanguages = GetApplyTemplateOptions(sourceXml, "btsl");
            FileTypes = GetApplyTemplateOptions(sourceXml, "ftts");
	        MatchRepairSettings = GetApplyTemplateOptions(sourceXml, "mrs");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTemplate"/> class.
        /// </summary>
        /// <param name="name">The template name.</param>
        public ApplyTemplate(string name)
        {
            Name = name;
            FileLocation = string.Empty;
            Uri = null;
            Id = Guid.Empty;
            TranslationProvidersAllLanguages = ApplyTemplateOptions.Keep;
            TranslationProvidersSpecificLanguages = ApplyTemplateOptions.Keep;
            TranslationMemoriesAllLanguages = ApplyTemplateOptions.Keep;
            TranslationMemoriesSpecificLanguages = ApplyTemplateOptions.Keep;
            TerminologyTermbases = ApplyTemplateOptions.Keep;
            TerminologySearchSettings = ApplyTemplateOptions.Keep;
            TranslationQualityAssessment = ApplyTemplateOptions.Keep;
            VerificationQaChecker30 = ApplyTemplateOptions.Keep;
            VerificationTagVerifier = ApplyTemplateOptions.Keep;
            VerificationTerminologyVerifier = ApplyTemplateOptions.Keep;
            VerificationGrammarChecker = ApplyTemplateOptions.Keep;
            VerificationNumberVerifier = ApplyTemplateOptions.Keep;
            BatchTasksAllLanguages = ApplyTemplateOptions.Keep;
            BatchTasksSpecificLanguages = ApplyTemplateOptions.Keep;
            FileTypes = ApplyTemplateOptions.Keep;
			MatchRepairSettings = ApplyTemplateOptions.Keep;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTemplate"/> class.
        /// </summary>
        /// <param name="templateInfo">The template information.</param>
        public ApplyTemplate(ProjectTemplateInfo templateInfo) : this("*" + templateInfo.Name)
        {
            Name = "*" + templateInfo.Name;
            Uri = templateInfo.Uri;
            Id = templateInfo.Id;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the file location.
        /// </summary>
        /// <value>
        /// The file location.
        /// </value>
        public string FileLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get => _templateName;

	        set
            {
                if (_templateName != value)
                {
                    _templateName = value;
	                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the translation providers all languages.
        /// </summary>
        /// <value>
        /// The translation providers all languages.
        /// </value>
        public ApplyTemplateOptions TranslationProvidersAllLanguages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the translation providers specific languages.
        /// </summary>
        /// <value>
        /// The translation providers specific languages.
        /// </value>
        public ApplyTemplateOptions TranslationProvidersSpecificLanguages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the translation memories all languages.
        /// </summary>
        /// <value>
        /// The translation memories all languages.
        /// </value>
        public ApplyTemplateOptions TranslationMemoriesAllLanguages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the translation memories specific languages.
        /// </summary>
        /// <value>
        /// The translation memories specific languages.
        /// </value>
        public ApplyTemplateOptions TranslationMemoriesSpecificLanguages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the behavior for terminology databases.
        /// </summary>
        /// <value>
        /// The terminology databases behavior.
        /// </value>
        public ApplyTemplateOptions TerminologyTermbases
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the terminology search settings.
        /// </summary>
        /// <value>
        /// The terminology search settings.
        /// </value>
        public ApplyTemplateOptions TerminologySearchSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the batch tasks all languages.
        /// </summary>
        /// <value>
        /// The batch tasks all languages.
        /// </value>
        public ApplyTemplateOptions BatchTasksAllLanguages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the batch tasks specific languages.
        /// </summary>
        /// <value>
        /// The batch tasks specific languages.
        /// </value>
        public ApplyTemplateOptions BatchTasksSpecificLanguages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the verification QA Checker 3.0 behavior.
        /// </summary>
        /// <value>
        /// The verification QA Checker 3.0 behavior.
        /// </value>
        public ApplyTemplateOptions VerificationQaChecker30
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the verification tag verifier behavior.
        /// </summary>
        /// <value>
        /// The verification tag verifier behavior.
        /// </value>
        public ApplyTemplateOptions VerificationTagVerifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the verification terminology verifier.
        /// </summary>
        /// <value>
        /// The verification terminology verifier.
        /// </value>
        public ApplyTemplateOptions VerificationTerminologyVerifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the verification number verifier.
        /// </summary>
        /// <value>
        /// The verification number verifier.
        /// </value>
        public ApplyTemplateOptions VerificationNumberVerifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the verification grammer checker.
        /// </summary>
        /// <value>
        /// The verification grammer checker.
        /// </value>
        public ApplyTemplateOptions VerificationGrammarChecker
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the file types.
        /// </summary>
        /// <value>
        /// The file types.
        /// </value>
        public ApplyTemplateOptions FileTypes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the translation quality assessment settings.
        /// </summary>
        /// <value>
        /// The translation quality assessment settings.
        /// </value>
        public ApplyTemplateOptions TranslationQualityAssessment
        {
            get;
            set;
        }

	    public ApplyTemplateOptions MatchRepairSettings { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("template");
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("location", FileLocation);
            writer.WriteAttributeString("id", Id.ToString("D"));
            writer.WriteAttributeString("tpal", TranslationProvidersAllLanguages.ToString());
            writer.WriteAttributeString("tpsl", TranslationProvidersSpecificLanguages.ToString());
            writer.WriteAttributeString("tmal", TranslationMemoriesAllLanguages.ToString());
            writer.WriteAttributeString("tmsl", TranslationMemoriesSpecificLanguages.ToString());
            writer.WriteAttributeString("tbtb", TerminologyTermbases.ToString());
            writer.WriteAttributeString("tqa", TranslationQualityAssessment.ToString());
            writer.WriteAttributeString("tbss", TerminologySearchSettings.ToString());
            writer.WriteAttributeString("qaqa", VerificationQaChecker30.ToString());
            writer.WriteAttributeString("qatg", VerificationTagVerifier.ToString());
            writer.WriteAttributeString("qatv", VerificationTerminologyVerifier.ToString());
            writer.WriteAttributeString("qanv", VerificationNumberVerifier.ToString());
            writer.WriteAttributeString("qagc", VerificationGrammarChecker.ToString());
            writer.WriteAttributeString("btal", BatchTasksAllLanguages.ToString());
            writer.WriteAttributeString("btsl", BatchTasksSpecificLanguages.ToString());
            writer.WriteAttributeString("ftts", FileTypes.ToString());
			writer.WriteAttributeString("mrs",MatchRepairSettings.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets the apply template options.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>The apply options for this specified attribute</returns>
        private ApplyTemplateOptions GetApplyTemplateOptions(XmlNode xmlNode, string attributeName)
        {
            var returnValue = ApplyTemplateOptions.Keep;
            if (((XmlElement)xmlNode).HasAttribute(attributeName))
            {
	            var selectSingleNode = xmlNode.SelectSingleNode("@" + attributeName);
	            if (selectSingleNode != null)
	            {
		            string attributeValue = selectSingleNode.Value;
		            if (Enum.TryParse(attributeValue, out returnValue))
		            {
			            if (!Enum.IsDefined(typeof(ApplyTemplateOptions), returnValue) && !returnValue.ToString().Contains(","))
			            {
				            returnValue = ApplyTemplateOptions.Keep;
			            }
		            }
	            }
            }

            return returnValue;
        }
    }
}
