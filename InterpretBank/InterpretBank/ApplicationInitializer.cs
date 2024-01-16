using Autofac;
using InterpretBank.Booth;
using InterpretBank.Booth.ViewModel;
using InterpretBank.CommonServices;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Interface;
using InterpretBank.SettingsService.UI;
using InterpretBank.SettingsService.ViewModel;
using InterpretBank.Studio;
using InterpretBank.TermbaseViewer.UI;
using InterpretBank.TermbaseViewer.ViewModel;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization.CultureMetadataManager;
using Sdl.Core.Globalization.CultureMetadataService;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace InterpretBank
{
    [ApplicationInitializer]
    public class ApplicationInitializer : IApplicationInitializer
    {
        public static ILifetimeScope ApplicationLifetimeScope { get; set; }
        private static ContainerBuilder Builder { get; } = new();

        public static void RegisterComponents()
        {
            RegisterGlobalDependencies();
            RegisterTermbaseViewerControl();
            RegisterInterpretBankProvider();
            RegisterSettingsUi();
            RegisterGlossarySetup();
            RegisterBooth();

            ApplicationLifetimeScope = Builder.Build().BeginLifetimeScope();
        }

        private static void RegisterBooth()
        {
            Builder.RegisterType<BoothWindowViewModel>();
            Builder.RegisterType<BoothWindow>();
        }

        public void Execute()
        {
           
            RegisterComponents();
        }

        private static void RegisterGlobalDependencies()
        {
            Builder.RegisterType<InterpretBankDataContext>().As<IInterpretBankDataContext>();
            Builder.RegisterType<TerminologyService.TerminologyService>().As<ITerminologyService>();
            Builder.RegisterType<UserInteractionService>().As<IUserInteractionService>();
            Builder.RegisterType<PersistenceService.PersistenceService>().SingleInstance();
            Builder.RegisterType<GlossaryExchangeService.GlossaryExchangeService>().SingleInstance();
        }

        private static void RegisterGlossarySetup()
        {
            Builder.RegisterType<GlossarySetupViewModel>();
            Builder.RegisterType<GlossarySetup>();
        }

        private static void RegisterInterpretBankProvider()
        {
            Builder.RegisterType<InterpretBankProvider>();
        }

        private static void RegisterSettingsUi()
        {
            Builder.RegisterType<SettingsService.ViewModel.SettingsService>();
            Builder.RegisterType<SettingsMain>();
        }

        private static void RegisterTermbaseViewerControl()
        {
            Builder.RegisterType<TermbaseViewerViewModel>().As<TermbaseViewerViewModel>();
            Builder.RegisterType<TermbaseViewer.UI.TermbaseViewer>().As<TermbaseViewer.UI.TermbaseViewer>();
            Builder.RegisterType<TermbaseViewerControl>().As<TermbaseViewerControl>();
        }
    }
}