using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Specifications.LocalizationSpecification
{
    public class PreventLocalizationSpecification: IExtractSpecification
    {
        public bool IsSatisfiedBy(IExtractData numberExtractResults)
        {
            return numberExtractResults.Settings.PreventLocalizations;
        }
    }
}
