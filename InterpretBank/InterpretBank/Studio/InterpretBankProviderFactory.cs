using System;
using System.Data.SQLite;
using InterpretBank.GlossaryService;
using InterpretBank.TerminologyService;
using InterpretBank.Wrappers;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
	[TerminologyProviderFactory(Id = "My_Terminology_Provider_Id",
								Name = "My_Terminology_Provider_Name",
								Description = "My_Terminology_Provider_Description")]
	public class InterpretBankProviderFactory : ITerminologyProviderFactory
	{
		public static InterpretBankProvider GetInterpretBankProvider()
		{
			var filepath = "C:\\Code\\RWS Community\\InterpretBank\\InterpretBankTests\\Resources\\InterpretBankDatabaseV6.db";
			var sqLiteConnection = new SQLiteConnection($"Data Source={filepath}");

			var interpretBankDataContext = new InterpretBankDataContext(sqLiteConnection);
			var settingsService = new SettingsService.ViewModel.SettingsService(new OpenFileDialog(), interpretBankDataContext);

			var termSearchService = new TerminologyService.TerminologyService(interpretBankDataContext);
			return new InterpretBankProvider(termSearchService, settingsService);
		}

		public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
		{
			return GetInterpretBankProvider();
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri == new Uri(Constants.InterpretBankUri);
		}
	}
}