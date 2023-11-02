using Autofac;
using InterpretBank.CommonServices;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Interface;
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
        public static ILifetimeScope Container { get; set; }
        private static ContainerBuilder Builder { get; } = new();

        public void Execute()
        {
            RegisterGlobalDependencies();
            RegisterTermbaseViewerControl();
            RegisterInterpretBankProvider();
            Container = Builder.Build().BeginLifetimeScope();
        }

        private static void RegisterTermbaseViewerControl()
        {
            Builder.RegisterType<TermbaseViewerViewModel>().As<TermbaseViewerViewModel>().InstancePerLifetimeScope();
            Builder.RegisterType<TermbaseViewer.UI.TermbaseViewer>().As<TermbaseViewer.UI.TermbaseViewer>().InstancePerLifetimeScope();
            Builder.RegisterType<TermbaseViewerControl>().As<TermbaseViewerControl>().InstancePerLifetimeScope();
        }

        private void RegisterGlobalDependencies()
        {
            Builder.RegisterType<InterpretBankDataContext>().As<IInterpretBankDataContext>().InstancePerLifetimeScope();
            Builder.RegisterType<TerminologyService.TerminologyService>().As<ITerminologyService>().InstancePerLifetimeScope();
            Builder.RegisterType<UserInteractionService>().As<IUserInteractionService>().SingleInstance();
        }

        private void RegisterInterpretBankProvider()
        {
            Builder.RegisterType<InterpretBankProvider>().InstancePerLifetimeScope();
        }
    }
}