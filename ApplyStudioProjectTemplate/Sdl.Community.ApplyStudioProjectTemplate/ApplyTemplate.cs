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
            this.Name = sourceXml.SelectSingleNode("@name").Value;
            this.FileLocation = sourceXml.SelectSingleNode("@location").Value;
            this.Uri = null;
            this.Id = new Guid(sourceXml.SelectSingleNode("@id").Value);
            this.TranslationProvidersAllLanguages = this.GetApplyTemplateOptions(sourceXml, "tpal");
            this.TranslationProvidersSpecificLanguages = this.GetApplyTemplateOptions(sourceXml, "tpsl");
            this.TranslationMemoriesAllLanguages = this.GetApplyTemplateOptions(sourceXml, "tmal");
            this.TranslationMemoriesSpecificLanguages = this.GetApplyTemplateOptions(sourceXml, "tmsl");
            this.TerminologyTermbases = this.GetApplyTemplateOptions(sourceXml, "tbtb");
            this.TerminologySearchSettings = this.GetApplyTemplateOptions(sourceXml, "tbss");
            this.TranslationQualityAssessment = this.GetApplyTemplateOptions(sourceXml, "tqa");
            this.VerificationQaChecker30 = this.GetApplyTemplateOptions(sourceXml, "qaqa");
            this.VerificationTagVerifier = this.GetApplyTemplateOptions(sourceXml, "qatg");
            this.VerificationTerminologyVerifier = this.GetApplyTemplateOptions(sourceXml, "qatv");
            this.VerificationNumberVerifier = this.GetApplyTemplateOptions(sourceXml, "qanv");
            this.VerificationGrammarChecker = this.GetApplyTemplateOptions(sourceXml, "qagc");
            this.BatchTasksAllLanguages = this.GetApplyTemplateOptions(sourceXml, "btal");
            this.BatchTasksSpecificLanguages = this.GetApplyTemplateOptions(sourceXml, "btsl");
            this.FileTypes = this.GetApplyTemplateOptions(sourceXml, "ftts");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTemplate"/> class.
        /// </summary>
        /// <param name="name">The template name.</param>
        public ApplyTemplate(string name)
        {
            this.Name = name;
            this.FileLocation = string.Empty;
            this.Uri = null;
            this.Id = Guid.Empty;
            this.TranslationProvidersAllLanguages = ApplyTemplateOptions.Keep;
            this.TranslationProvidersSpecificLanguages = ApplyTemplateOptions.Keep;
            this.TranslationMemoriesAllLanguages = ApplyTemplateOptions.Keep;
            this.TranslationMemoriesSpecificLanguages = ApplyTemplateOptions.Keep;
            this.TerminologyTermbases = ApplyTemplateOptions.Keep;
            this.TerminologySearchSettings = ApplyTemplateOptions.Keep;
            this.TranslationQualityAssessment = ApplyTemplateOptions.Keep;
            this.VerificationQaChecker30 = ApplyTemplateOptions.Keep;
            this.VerificationTagVerifier = ApplyTemplateOptions.Keep;
            this.VerificationTerminologyVerifier = ApplyTemplateOptions.Keep;
            this.VerificationGrammarChecker = ApplyTemplateOptions.Keep;
            this.VerificationNumberVerifier = ApplyTemplateOptions.Keep;
            this.BatchTasksAllLanguages = ApplyTemplateOptions.Keep;
            this.BatchTasksSpecificLanguages = ApplyTemplateOptions.Keep;
            this.FileTypes = ApplyTemplateOptions.Keep;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyTemplate"/> class.
        /// </summary>
        /// <param name="templateInfo">The template information.</param>
        public ApplyTemplate(ProjectTemplateInfo templateInfo) : this("*" + templateInfo.Name)
        {
            this.Name = "*" + templateInfo.Name;
            this.Uri = templateInfo.Uri;
            this.Id = templateInfo.Id;
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
            get
            {
                return this._templateName;
            }

            set
            {
                if (this._templateName != value)
                {
                    this._templateName = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                    }
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

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("template");
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("location", this.FileLocation);
            writer.WriteAttributeString("id", this.Id.ToString("D"));
            writer.WriteAttributeString("tpal", this.TranslationProvidersAllLanguages.ToString());
            writer.WriteAttributeString("tpsl", this.TranslationProvidersSpecificLanguages.ToString());
            writer.WriteAttributeString("tmal", this.TranslationMemoriesAllLanguages.ToString());
            writer.WriteAttributeString("tmsl", this.TranslationMemoriesSpecificLanguages.ToString());
            writer.WriteAttributeString("tbtb", this.TerminologyTermbases.ToString());
            writer.WriteAttributeString("tqa", this.TranslationQualityAssessment.ToString());
            writer.WriteAttributeString("tbss", this.TerminologySearchSettings.ToString());
            writer.WriteAttributeString("qaqa", this.VerificationQaChecker30.ToString());
            writer.WriteAttributeString("qatg", this.VerificationTagVerifier.ToString());
            writer.WriteAttributeString("qatv", this.VerificationTerminologyVerifier.ToString());
            writer.WriteAttributeString("qanv", this.VerificationNumberVerifier.ToString());
            writer.WriteAttributeString("qagc", this.VerificationGrammarChecker.ToString());
            writer.WriteAttributeString("btal", this.BatchTasksAllLanguages.ToString());
            writer.WriteAttributeString("btsl", this.BatchTasksSpecificLanguages.ToString());
            writer.WriteAttributeString("ftts", this.FileTypes.ToString());
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
            ApplyTemplateOptions returnValue = ApplyTemplateOptions.Keep;
            if (((XmlElement)xmlNode).HasAttribute(attributeName))
            {
                string attributeValue = xmlNode.SelectSingleNode("@" + attributeName).Value;
                if (Enum.TryParse(attributeValue, out returnValue))
                {
                    if (!Enum.IsDefined(typeof(ApplyTemplateOptions), returnValue) && !returnValue.ToString().Contains(","))
                    {
                        returnValue = ApplyTemplateOptions.Keep;
                    }
                }
            }

            return returnValue;
        }
    }
}
