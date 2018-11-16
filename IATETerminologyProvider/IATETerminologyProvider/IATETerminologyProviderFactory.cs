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
			IATETerminologyProvider terminologyProvider;
			try
			{
				var persistenceService = new PersistenceService();

				var providerSettings = persistenceService.Load();
				//in case we didn't any settings stored there is no need to load the provider
				if (providerSettings == null)
				{
					return null;
				}

				terminologyProvider = new IATETerminologyProvider(providerSettings);
				//Task.Run(terminologyProvider.LoadEntries);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return terminologyProvider;
		}
		#endregion
	}
}