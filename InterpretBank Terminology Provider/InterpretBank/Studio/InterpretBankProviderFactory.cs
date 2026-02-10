using Autofac;
using Sdl.Terminology.TerminologyProvider.Core;
using System;

namespace InterpretBank.Studio
{
    [TerminologyProviderFactory(Id = nameof(InterpretBankProviderFactory),
                                Name = "My_Terminology_Provider_Name",
                                Description = "My_Terminology_Provider_Description")]
    public class InterpretBankProviderFactory : ITerminologyProviderFactory
    {
        private ILifetimeScope TerminologyProviderScope { get; } =
            ApplicationInitializer.ApplicationLifetimeScope.BeginLifetimeScope();

        public ITerminologyProvider CreateTerminologyProvider(Uri terminologyProviderUri)
        {
            var settingsId = terminologyProviderUri.AbsolutePath.Split('.')[0].TrimStart('/');
            var settings = TerminologyProviderScope.Resolve<PersistenceService.PersistenceService>().GetSettings(settingsId);

            var interpretBankProvider = TerminologyProviderScope.Resolve<InterpretBankProvider>();
            interpretBankProvider.Setup(settings);

            return interpretBankProvider;
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri) =>
            terminologyProviderUri.ToString().Contains(Constants.InterpretBankUri);
    }
}