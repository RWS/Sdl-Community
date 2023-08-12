using System.Globalization;
using LanguageWeaverProvider.ViewModel;
using Sdl.Core.Globalization;

namespace LanguageWeaverProvider.Model
{
	public class PairMapping : BaseViewModel
	{
		public PairMapping(CultureCode sourceCode, CultureCode targetCode)
		{
			SourceCultureInfo = new CultureInfo(sourceCode.Name);
			TargetCultureInfo = new CultureInfo(targetCode.Name);

			DisplayName = $"{SourceCultureInfo.DisplayName} - {TargetCultureInfo.DisplayName}";


			// temp
			SourceCode = SourceCultureInfo.ThreeLetterISOLanguageName;
			TargetCode = TargetCultureInfo.ThreeLetterISOLanguageName;
			// temp
		}

		public string DisplayName { get; private set; }

		public CultureInfo SourceCultureInfo { get; private set; }

		public CultureInfo TargetCultureInfo { get; private set; }

		public string SourceCode { get; set; }

		public string TargetCode { get; set; }
	}
}