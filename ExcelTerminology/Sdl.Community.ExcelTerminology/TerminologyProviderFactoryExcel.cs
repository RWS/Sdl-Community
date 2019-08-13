using System;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology
{
	[TerminologyProviderFactory(Id = "TerminologyProviderFactoryExcel", Name = "Excel Terminology Provider", Description = "Excel terminology provider factory")]
	public class TerminologyProviderFactoryExcel : ITerminologyProviderFactory
	{
		public static readonly Log Log = Log.Instance;

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "excelglossary";
		}

		public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri,
			ITerminologyProviderCredentialStore credentials)
		{

			TerminologyProviderExcel terminologyProvider;
			try
			{
				var persistenceService = new PersistenceService();

				var providerSettings = persistenceService.Load(terminologyProviderUri);

				//in case we didn't any settings stored there is no need to load the provider
				if (providerSettings == null)
				{
					return null;
				}

				var termSearchService = new NormalTermSeachService(providerSettings);
				terminologyProvider = new TerminologyProviderExcel(providerSettings, termSearchService);
				Task.Run(terminologyProvider.LoadEntries);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateTerminologyProvider method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
			return terminologyProvider;
		}
	}
}