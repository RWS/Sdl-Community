using System.Collections.Generic;
using System.Xml.Serialization;

namespace VerifyFilesAuditReport.Components.SettingsProvider.Model;

[XmlRoot("VerificationSettings")]
public class VerificationSettingsTreeNode
{
    [XmlAttribute]
    public string Value { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlElement("Setting")]
    public List<VerificationSettingsTreeNode> Values { get; set; } = [];

    public VerificationSettingsTreeNode FindSettingValueRecursive(string valueName, VerificationSettingsTreeNode node = null)
    {
        node ??= this;

        if (node.Name == valueName)
            return node;

        if (node.Values == null)
            return null;

        foreach (var value in node.Values)
        {
            var found = FindSettingValueRecursive(valueName, value);
            if (found != null)
                return found;
        }

        return null;
    }
}
