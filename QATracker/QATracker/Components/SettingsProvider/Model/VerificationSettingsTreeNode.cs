using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace QATracker.Components.SettingsProvider.Model;

[XmlRoot("VerificationSettings")]
public class VerificationSettingsTreeNode
{
    [XmlAttribute] 
    public string Name { get; set; }

    [XmlElement("Setting")] 
    public List<VerificationSettingValue> Values { get; set; } = [];

    [XmlElement("Settings")] 
    public List<VerificationSettingsTreeNode> Children { get; set; } = [];

    
}