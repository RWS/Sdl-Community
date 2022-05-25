using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit
{
	[ApplicationInitializer]
	public class TransitApplicationInitializer : IApplicationInitializer
	{
		public static IStudioEventAggregator EventAggregator { get; } = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			                                                                
		public void Execute()
		{
			Log.Setup();
		}
	}
}
