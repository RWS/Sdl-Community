using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Sdl.Core.Globalization;

namespace Sdl.Community.TMOptimizerLib
{
    public class Detector
    {
        private readonly string _inputFile;
        
        public Detector(string inputFile)
        {
            _inputFile = inputFile;
        }

        public DetectInfo Detect()
        {
            var detectInfo = new DetectInfo();
            bool languagesDetected = false;
            using (var reader = XmlReader.Create(_inputFile, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "header" && !string.IsNullOrEmpty(reader.GetAttribute("o-tmf")))
                            {
                                switch (reader.GetAttribute("o-tmf"))
                                {
                                    case "TW4Win 2.0 Format":
                                        detectInfo.DetectedVersion = DetectInfo.Versions.Workbench;
                                        break;
                                    case "SDL TM8 Format":
                                        detectInfo.DetectedVersion = DetectInfo.Versions.Studio;
                                        break;
                                    default:
                                        detectInfo.DetectedVersion = DetectInfo.Versions.Unknown;
                                        break;
                                }
                            }

                            if (reader.Name == "tu")
                            {
                                if (!languagesDetected)
                                {
                                    var el = XNode.ReadFrom(reader) as XElement;
                                    if (el != null && el.Elements("tuv").Count() > 1)
                                    {
                                        detectInfo.OriginalSourceLanguage = el.Elements("tuv").First().Attribute((XNamespace.Xml + "lang")).Value;
                                        detectInfo.SourceLanguage = ConvertToLanguage(detectInfo.OriginalSourceLanguage, detectInfo.DetectedVersion);
                                        detectInfo.OriginalTargetLanguage = el.Elements("tuv").Last().Attribute((XNamespace.Xml + "lang")).Value;
                                        detectInfo.TargetLanguage = ConvertToLanguage(detectInfo.OriginalTargetLanguage, detectInfo.DetectedVersion);
                                        languagesDetected = true;
                                    }
                                }
                                else
                                {
                                    // don't read anything below the tu element
                                    reader.Skip();
                                }

                                detectInfo.TuCount++;
                                
                            }
                            break;
                    }
                }
            }

            return detectInfo;

        }

        private Language ConvertToLanguage(string languageCode, DetectInfo.Versions tmxVersion)
        {
            switch (tmxVersion)
            {
                case DetectInfo.Versions.Studio:
                    return new Language(languageCode);
                case DetectInfo.Versions.Workbench:
                    int lcid = LegacyTradosLanguage.GetLcidFromIsoCode(languageCode);
                    return lcid > 0 ? new Language(CultureInfo.GetCultureInfo(lcid)) : new Language(languageCode);
                case DetectInfo.Versions.Unknown:
                    return new Language(languageCode);
                default:
                    throw new ArgumentException("Unknown TMX version: " + tmxVersion);
            }
        }


    }
}
