using Autofac;
using InterpretBank.CommonServices;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Interface;
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
        private static ContainerBuilder Builder { get; } = new();

        public static ILifetimeScope Container { get; set; } 

        public void Execute()
        {
            Builder.RegisterType<InterpretBankDataContext>().As<IInterpretBankDataContext>().InstancePerLifetimeScope();

            Builder.RegisterType<TerminologyService.TerminologyService>().As<ITerminologyService>();

            RegisterTermbaseViewerControl();
            Container = Builder.Build().BeginLifetimeScope();
        }

        private static void RegisterTermbaseViewerControl()
        {
            Builder.RegisterType<TermbaseViewerViewModel>().As<TermbaseViewerViewModel>().InstancePerLifetimeScope();
            Builder.RegisterType<TermbaseViewer.UI.TermbaseViewer>().As<TermbaseViewer.UI.TermbaseViewer>().InstancePerLifetimeScope();
            Builder.RegisterType<TermbaseViewerControl>().As<TermbaseViewerControl>().InstancePerLifetimeScope();
        }
    }
}