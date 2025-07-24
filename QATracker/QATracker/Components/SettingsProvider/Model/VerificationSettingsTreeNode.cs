using System.Collections.Generic;
using System.Xml.Serialization;

namespace QATracker.Components.SettingsProvider.Model;

[XmlRoot("VerificationSettings")]
public class VerificationSettingsTreeNode
{
    [XmlAttribute]
    public string Enabled { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlText]
    public string Value { get; set; }

    [XmlElement("Setting")]
    public List<VerificationSettingsTreeNode> Values { get; set; } = [];

    public VerificationSettingsTreeNode FindSettingValueRecursive(string valueName, VerificationSettingsTreeNode node = null)
    {
        node ??= this;

        if (node.Values != null)
        {
            foreach (var value in node.Values)
            {
                if (value.Name == valueName)
                    return value;

                if (value.Values != null)
                {
                    var foundInValue = FindSettingValueInValueRecursive(valueName, value);
                    if (foundInValue != null)
                        return foundInValue;
                }
            }
        }

        return null;
    }

    private VerificationSettingsTreeNode FindSettingValueInValueRecursive(string valueName, VerificationSettingsTreeNode value)
    {
        if (value.Values != null)
        {
            foreach (var nestedValue in value.Values)
            {
                if (nestedValue.Name == valueName)
                    return nestedValue;

                var found = FindSettingValueInValueRecursive(valueName, nestedValue);
                if (found != null)
                    return found;
            }
        }
        return null;
    }
}