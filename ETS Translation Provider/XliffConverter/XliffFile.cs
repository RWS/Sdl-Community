using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace XliffConverter
{
    public class TargetTranslation
    {
        public TargetTranslation() { }
        public TargetTranslation(CultureInfo targetCulture, string text)
        {
            TargetCulture = targetCulture;
            Text = text;
        }

        #region Culture Info

        [XmlAttribute("xml:lang")]
        public string TargetLanguage
        {
            get
            {
                return _targetLanguage;
            }
            set
            {
                _targetLanguage = value;

                // verify TargetCulture isn't the desired value before setting,
                // else you run the risk of infinite loop
                CultureInfo newTargetCulture = CultureInfo.GetCultureInfo(value);
                if (TargetCulture != newTargetCulture)
                    TargetCulture = newTargetCulture;
            }
        }

        [XmlIgnore]
        private string _targetLanguage;

        [XmlIgnore]
        public CultureInfo TargetCulture { get
            {
                return _targetCulture;
            }
            set
            {
                _targetCulture = value;

                // verify targetLanguage isn't the desired value before setting,
                // else you run the risk of infinite loop
                string newTargetLanguage = value.ToString();
                if (TargetLanguage != newTargetLanguage)
                    TargetLanguage = newTargetLanguage;
            }
        }

        [XmlIgnore]
        private CultureInfo _targetCulture;

        #endregion

        [XmlText()]
        public string Text { get; set; }

        [XmlIgnore]
        public Segment TargetSegment
        {
            get
            {
                return SegmentParser.Parser.ParseLine(Converter.removeXliffTags(Text));
            }
        }
    }

    public class TranslationOption
    {
        public TranslationOption() { }
        public TranslationOption(string toolID, TargetTranslation translation)
        {
            ToolID = toolID;
            Translation = translation;
        }

        [XmlAttribute("tool-id")]
        public string ToolID { get; set; }

        [XmlAttribute("date")]
        public string Date{ get; set; }

        [XmlElement("target")]
        public TargetTranslation Translation { get; set; }

    }
    public class TranslationUnit
    {
        [XmlAttribute("id")]
        public int ID { get; set; }

        [XmlElement("source")]
        public string SourceText { get; set; }

        [XmlElement("alt-trans")]
        public List<TranslationOption> TranslationList { get; set; }

    }

    public class Body
    {
        [XmlElement("trans-unit")]
        public List<TranslationUnit> TranslationUnits { get; set; }

        public Body()
        {
            TranslationUnits = new List<TranslationUnit>();
        }
        internal void Add(Segment sourceSegment)
        {
            if (sourceSegment == null)
                throw new NullReferenceException("Source segment cannot be null");

            TranslationUnits.Add(new TranslationUnit()
            {
                ID = TranslationUnits.Count,
                SourceText = sourceSegment.ToXliffString(),
            });
        }

        internal void Add(Segment sourceSegment, Segment targetSegment, string toolID)
        {
            if (sourceSegment == null)
                throw new NullReferenceException("Source segment cannot be null");
            if (targetSegment == null)
                throw new NullReferenceException("Target segment cannot be null");

            TranslationUnits.Add(new TranslationUnit()
            {
                ID = TranslationUnits.Count,
                SourceText = sourceSegment.ToXliffString(),
                TranslationList = new List<TranslationOption>(){
                    new TranslationOption(
                        toolID,
                        new TargetTranslation(targetSegment.Culture, targetSegment.ToXliffString()))
                }

            });
        }
    }
    public class Tool
    {
        [XmlAttribute("tool-name")]
        public string ToolName { get; set; }

        [XmlAttribute("tool-version")]
        public string ToolVersion { get; set; }

        [XmlAttribute("tool-id")]
        public string ToolID { get; set; }
    }

    public class Header
    {
        [XmlElement("tool")]
        public Tool[] Tools { get; set; }
    }
    public class File
    {
        #region Culture Info
        [XmlAttribute("source-language")]
        public string SourceLanguage
        {
            get
            {
                return _sourceLanguage;
            }
            set
            {
                _sourceLanguage = value;

                // verify TargetCulture isn't the desired value before setting,
                // else you run the risk of infinite loop
                CultureInfo newSourceCulture = CultureInfo.GetCultureInfo(value);
                if (SourceCulture != newSourceCulture)
                    SourceCulture = newSourceCulture;
            }
        }

        [XmlIgnore]
        private string _sourceLanguage;

        [XmlIgnore]
        public CultureInfo SourceCulture { get; set; }

        [XmlAttribute("target-language")]
        public string TargetLanguage
        {
            get
            {
                return _targetLanguage;
            }
            set
            {
                _targetLanguage = value;

                // verify TargetCulture isn't the desired value before setting,
                // else you run the risk of infinite loop
                CultureInfo newTargetCulture = CultureInfo.GetCultureInfo(value);
                if (TargetCulture != newTargetCulture)
                    TargetCulture = newTargetCulture;
            }
        }

        [XmlIgnore]
        private string _targetLanguage;
        [XmlIgnore]
        public CultureInfo TargetCulture
        {
            get
            {
                return _targetCulture;
            }
            set
            {
                _targetCulture = value;

                // verify targetLanguage isn't the desired value before setting,
                // else you run the risk of infinite loop
                string newTargetLanguage = value.ToString();
                if (TargetLanguage != newTargetLanguage)
                    TargetLanguage = newTargetLanguage;
            }
        }

        [XmlIgnore]
        private CultureInfo _targetCulture;
        #endregion

        [XmlElement("header")]
        public Header Header { get; set; }

        [XmlElement("body")]
        public Body Body { get; set; }
        public File() { }

        public File(CultureInfo sourceCulture, CultureInfo targetCulture)
        {
            SourceCulture = sourceCulture;
            SourceCulture = targetCulture;
            Body = new Body();
        }
    }

    [XmlRoot(Namespace = "urn:oasis:names:tc:xliff:document:1.2")]
    public class xliff
    {
        [XmlAttribute]
        public string Version { get; set; }

        [XmlElement("file")]
        public File File { get; set; }

        public xliff() { }
        public xliff(CultureInfo sourceCulture, CultureInfo targetCulture, bool encodeUTF16 = false)
        {
            File = new File(sourceCulture, targetCulture);
        }

        public void AddTranslationUnit(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit translationUnit, string toolID)
        {
            File.Body.Add(translationUnit.SourceSegment, translationUnit.TargetSegment, toolID);
        }

        public void AddSourceText(string sourceText)
        {
            if (sourceText == null)
                throw new NullReferenceException("Source text cannot be null");
            Segment seg = new Segment();
            seg.Add(sourceText);
            File.Body.Add(seg);
        }

        public void AddSourceSegment(Segment sourceSegment)
        {
            File.Body.Add(sourceSegment);
        }

        public void AddTranslation(Segment sourceSegment, Segment targetSegment, string toolID)
        {
            File.Body.Add(sourceSegment, targetSegment, toolID);
        }

        public Segment[] GetTargetSegments()
        {
            return File.Body.TranslationUnits.Select(x =>
            {
                if (x.TranslationList == null) return null;
                TranslationOption option = x.TranslationList.FirstOrDefault();
                if (option == null) return null;
                if (option.Translation == null) return null;
                var segment = option.Translation.TargetSegment;
                if (segment == null) return null;

                segment.Culture = File.SourceCulture;
                return segment;
            }).ToArray();
        }

        public override string ToString()
        {
            return Converter.PrintXliff(this);
        }
    }
}
