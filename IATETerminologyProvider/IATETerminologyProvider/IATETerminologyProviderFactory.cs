using System;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider
{
	[TerminologyProviderFactory(Id = "IATETerminologyProvider", 
		Name = "IATE Terminology Provider", 
		Icon = "Iate_logo", 
		Description = "IATE terminology provider factory")]
	public class IATETerminologyProviderFactory : ITerminologyProviderFactory
	{
		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == Constants.IATEGlossary;
		}

		public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
		{
			var savedSettings = new SettingsModel(terminologyProviderUri);
			
			var terminologyProvider = new IATETerminologyProvider(savedSettings, 
				IATEApplication.ConnectionProvider, IATEApplication.InventoriesProvider);
			
			return terminologyProvider;
		}
	}
}