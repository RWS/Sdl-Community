using System;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank
{
	[TerminologyProviderFactory(Id = "My_Terminology_Provider_Id",
								Name = "My_Terminology_Provider_Name",
								Description = "My_Terminology_Provider_Description")]
	public class InterpretBankProviderFactory : ITerminologyProviderFactory
	{
		public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
		{
			throw new NotImplementedException();
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			throw new NotImplementedException();
		}
	}
}