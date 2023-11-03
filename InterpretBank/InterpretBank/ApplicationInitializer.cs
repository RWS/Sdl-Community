using Autofac;
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

            ApplicationLifetimeScope = Builder.Build().BeginLifetimeScope();
        }

        public void Execute()
        {
            RegisterComponents();
        }

        private static void RegisterGlobalDependencies()
        {
            Builder.RegisterType<InterpretBankDataContext>().As<IInterpretBankDataContext>().InstancePerDependency();

            Builder.RegisterType<TerminologyService.TerminologyService>().As<ITerminologyService>().InstancePerLifetimeScope();

            Builder.RegisterType<UserInteractionService>().As<IUserInteractionService>().SingleInstance();
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
            Builder.RegisterType<InterpretBankProvider>().InstancePerLifetimeScope();
        }

        private static void RegisterSettingsUi()
        {
            Builder.RegisterType<SettingsService.ViewModel.SettingsService>().InstancePerLifetimeScope();
            Builder.RegisterType<SettingsMain>().InstancePerLifetimeScope();
        }

        private static void RegisterTermbaseViewerControl()
        {
            Builder.RegisterType<TermbaseViewerViewModel>().As<TermbaseViewerViewModel>().InstancePerLifetimeScope();
            Builder.RegisterType<TermbaseViewer.UI.TermbaseViewer>().As<TermbaseViewer.UI.TermbaseViewer>().InstancePerLifetimeScope();
            Builder.RegisterType<TermbaseViewerControl>().As<TermbaseViewerControl>().InstancePerLifetimeScope();
        }
    }
}