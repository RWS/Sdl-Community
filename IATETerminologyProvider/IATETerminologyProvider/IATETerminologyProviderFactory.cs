using System;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider
{
	[TerminologyProviderFactory(Id = "IATETerminologyProvider", Name = "IATE Terminology Provider", Icon = "Iate_logo", Description = "IATE terminology provider factory")]
	public class IATETerminologyProviderFactory : ITerminologyProviderFactory
	{
		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == Constants.IATEGlossary;
		}

		public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
		{
			var savedSettings = new SettingsModel(terminologyProviderUri);

			var terminologyProvider = new IATETerminologyProvider(savedSettings);
			return terminologyProvider;
		}
	}
}