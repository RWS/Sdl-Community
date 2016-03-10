using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ExcelTerminology.Insights;
using Sdl.Community.ExcelTerminology.Services;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology
{
    [TerminologyProviderFactory(Id = "TerminologyProviderFactoryExcel",Name = "Excel Terminology Provider",Description = "Excel terminology provider factory")]
    public class TerminologyProviderFactoryExcel : ITerminologyProviderFactory
    {
        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return true;
        }

        public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri,
            ITerminologyProviderCredentialStore credentials)
        {
            TelemetryService.Instance.Init();
            TerminologyProviderExcel terminologyProvider = null;
            try
            {
                var persistenceService = new PersistenceService();

                var providerSettings = persistenceService.Load(terminologyProviderUri);
                //in case we didn't any settings stored there is no need to load the provider
                if (providerSettings == null)
                {
                    TelemetryService.Instance.AddEvent("No settings stored for the provider",
                        new Dictionary<string, string> { { "Uri", terminologyProviderUri.ToString() } });
                    return terminologyProvider;
                }
                var termSearchService = new NormalTermSeachService(providerSettings);

                terminologyProvider = new TerminologyProviderExcel(providerSettings, termSearchService);
                Task.Run(terminologyProvider.LoadEntries);
            }
            catch (Exception ex)
            {
                TelemetryService.Instance.AddException(ex);
            }
            return terminologyProvider;

        }
    }
}
