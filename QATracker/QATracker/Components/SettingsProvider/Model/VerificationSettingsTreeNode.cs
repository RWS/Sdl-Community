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


    // Recursively searches the tree for a VerificationSettingValue with the given name
    public VerificationSettingValue FindSettingValueRecursive(string valueName, VerificationSettingsTreeNode node = null)
    {
        node ??= this;

        var found = node.Values?.FirstOrDefault(v => v.Name == valueName);
        if (found != null)
            return found;

        return node.Children?.Select(child => FindSettingValueRecursive(valueName, child))
            .FirstOrDefault(foundInChild => foundInChild != null);
    }
}