using System;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.IATETerminologyProvider
{
	[ApplicationInitializer]
	public class IateApplicationInitializer: IApplicationInitializer
	{
		public void Execute()
		{
			var eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);
		}

		private async void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent e)
		{
			var domanService = new DomainService();
			var termTypeService = new TermTypeService();
			await domanService.GetDomains();
			await termTypeService.GetTermTypes();
		}
	}
}
