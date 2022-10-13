using System.Collections.Generic;
using Multilingual.Excel.FileType.Verifier.Settings;
using Sdl.Verification.Api;

namespace Multilingual.Excel.FileType.Verifier
{
	[LanguageDirectionVerifier(SettingsConstants.MultilingualExcelVerifierId, "Plugin_Name", "Plugin_Description")]
	public class MultilingualExcelVerifierLanguage: MultilingualExcelVerifier, ILanguageDirectionVerifier
	{
		public IList<string> GetLanguageDirectionSettingsPageExtensionIds()
		{
			IList<string> list = new List<string>
			{
				SettingsConstants.MultilingualExcelVerificationSettingsId
			};

			return list;
		}		
	}
}
