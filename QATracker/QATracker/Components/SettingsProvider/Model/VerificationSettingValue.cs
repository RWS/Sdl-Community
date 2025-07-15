using System.Xml.Serialization;

namespace QATracker.Components.SettingsProvider.Model
{
    public class VerificationSettingValue
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}