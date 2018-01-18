using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Windows.Forms;
using System.Reflection;

namespace Sdl.Community.TMLifting
{
    [View(
        Id = "TMLifting",
        Name = "TM Lifting",
        Description = "",
        Icon = "icon",
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
    public class TMLiftingRibbon : AbstractViewController
    {
        private readonly Lazy<TMLiftingForm> _viewContent = new Lazy<TMLiftingForm>();
        protected override Control GetContentControl()
        {
            return _viewContent.Value;
        }
        protected override void Initialize(IViewContext context)
        {
			
			string resourceGSKit = "Sdl.Community.TMLifting.GSKit.Sdl.Community.GroupShareKit.dll";
			string resourceSystemNetHttp = "Sdl.Community.TMLifting.GSKit.System.Net.Http.dll";
			string resourceSdlTmServiceSdk = "Sdl.Community.TMLifting.GSKit.Sdl.TmService.Sdk.dll";
			string resourceNewtonsoftJson = "Sdl.Community.TMLifting.GSKit.Newtonsoft.Json.dll";			
			EmbeddedAssembly.Load(resourceGSKit, "Sdl.Community.GroupShareKit.dll");
			EmbeddedAssembly.Load(resourceSystemNetHttp, "System.Net.Http.dll");
			EmbeddedAssembly.Load(resourceSdlTmServiceSdk, "Sdl.TmService.Sdk.dll");
			EmbeddedAssembly.Load(resourceNewtonsoftJson, "Newtonsoft.Json.dll");
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}
		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return EmbeddedAssembly.Get(args.Name);
		}
	}
}