using System;
using IATETerminologyProvider.Service;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider
{
	[TerminologyProviderFactory(Id = "IATETerminologyProvider",	Name = "IATE Terminology Provider", Description = "IATE terminology provider factory")]
	public class IATETerminologyProviderFactory : ITerminologyProviderFactory
	{		
		#region Public Methodss
		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "iateglossary";
		}

		public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
		{
			GetDomains();
			IATETerminologyProvider terminologyProvider;
			try
			{
				var persistenceService = new PersistenceService();
				var providerSettings = persistenceService.Load();

				terminologyProvider = new IATETerminologyProvider(providerSettings);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return terminologyProvider;
		}

		private void GetDomains()
		{
			DomainService.GetDomains();
		}
		#endregion
	}
}