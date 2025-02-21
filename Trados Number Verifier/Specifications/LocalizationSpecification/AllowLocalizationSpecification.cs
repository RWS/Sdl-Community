using Sdl.Community.NumberVerifier.Interfaces;

namespace Sdl.Community.NumberVerifier.Specifications.LocalizationSpecification
{
    public class AllowLocalizationSpecification:IExtractSpecification
    {

        public bool IsSatisfiedBy(IExtractData numberExtractResults)
        {
            return numberExtractResults.Settings.AllowLocalizations;
        }
    }
}
