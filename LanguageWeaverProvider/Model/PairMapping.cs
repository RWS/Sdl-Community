using System.Globalization;
using LanguageMappingProvider.Model;
using LanguageWeaverProvider.ViewModel;
using Sdl.Core.Globalization;

namespace LanguageWeaverProvider.Model
{
	public class PairMapping : BaseViewModel
	{
		public PairMapping(LanguageMapping sourceCode, LanguageMapping targetCode)
		{
			DisplayName = $"{sourceCode.Name} ({sourceCode.Region} - {targetCode.Name} ({targetCode.Region}";

			SourceCode = sourceCode.LanguageCode;
			TargetCode = targetCode.LanguageCode;
		}

		public string DisplayName { get; private set; }

		public string SourceCode { get; set; }

		public string TargetCode { get; set; }
	}
}