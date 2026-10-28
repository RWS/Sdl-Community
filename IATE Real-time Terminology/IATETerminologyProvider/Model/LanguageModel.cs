using System.Globalization;
using TradosStudio.API.TranslationResources.Terminology;


namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class LanguageModel : ILanguage
	{
		public string Name { get; set; }
		public CultureInfo CultureInfo { get; set; }
        public string LanguageIsoCode { 
            get {
                //CultureInfo.Name is the ISO code of the language ex. fr-FR  
                return CultureInfo.Name;                                   
            } 
        }
     }
}