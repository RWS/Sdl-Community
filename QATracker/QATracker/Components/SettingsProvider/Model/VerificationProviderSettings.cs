using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Model;

public class VerificationProviderSettings
{
    public Dictionary<string, VerificationSettingsTreeNode> LanguageVerificationProviders { get; set; }
    public VerificationSettingsTreeNode ProjectVerificationProviders { get; set; }
}