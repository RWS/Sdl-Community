using System.Collections.Generic;

namespace QATracker.Components.SettingsProvider.Verifiers.Interface
{
    public interface IVerificationSettings
    {
        List<string> CategoriesList { get; set; }
        Dictionary<string, object> DefaultSubcategoryValuesMap { get; set; }
        string Id { get; set; }
        string Name { get; set; }
        Dictionary<string, string> SdlprojStringToUiStringMap { get; set; }
        Dictionary<string, List<string>> SubcategoriesMap { get; set; }
        Dictionary<string, Dictionary<string, string>> UiStringToSdlprojStringMap { get; set; }
    }
}