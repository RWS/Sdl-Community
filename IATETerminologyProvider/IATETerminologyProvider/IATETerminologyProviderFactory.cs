using System;
using NLog;
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
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		
		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == Constants.IATEGlossary;
		}

		public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri, ITerminologyProviderCredentialStore credentials)
		{
			var savedSettings = new SettingsModel(terminologyProviderUri);

			if (!IATEApplication.ConnectionProvider.EnsureConnection())
			{
				var exception = new Exception("Failed login!");
				_logger.Error(exception);

				throw exception;
			}
			
			var terminologyProvider = new IATETerminologyProvider(savedSettings, 
				IATEApplication.ConnectionProvider, IATEApplication.InventoriesProvider);
			
			return terminologyProvider;
		}
	}
}