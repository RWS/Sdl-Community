using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Model;

public class VerificationProviderSettings
{
    public Dictionary<string, List<dynamic>> LanguageVerificationProviders { get; set; }
    public List<dynamic> ProjectVerificationProviders { get; set; }
}