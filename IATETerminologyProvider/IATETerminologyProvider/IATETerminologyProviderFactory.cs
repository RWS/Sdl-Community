using System;
using System.Windows.Forms;
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
			if (DomainService.Domains.Count == 0)
			{
				GetDomains();
			}
			IATETerminologyProvider terminologyProvider;
			try
			{
				var persistenceService = new PersistenceService();
				var providerSettings = persistenceService.Load();
				//in case we didn't any settings stored there is no need to load the provider and display message
				if (providerSettings == null)
				{
					MessageBox.Show(string.Empty, "No settings are done on IATE provider! Please remove and add again provider.", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return null;
				}
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