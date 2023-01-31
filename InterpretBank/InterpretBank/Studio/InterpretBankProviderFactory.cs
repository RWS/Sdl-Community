using System;
using System.Collections.Generic;
using InterpretBank.TermSearch;
using InterpretBankTests;
using NSubstitute;
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
			//var sqlGlossaryService = new SqlGlossaryService(new DatabaseConnection("file"), new SqlBuilder());
			//var settingsService = new SettingsService();

			//var termSearchService = new TermSearchService(sqlGlossaryService, settingsService);
			var mg = new MockGenerator();
			return new InterpretBankProvider(mg.GetTermSearchService());
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