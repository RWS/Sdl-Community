using System;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class EventAggregatorService: IEventAggregatorService
	{
		private readonly IStudioEventAggregator _studioEventAggregator;
		public EventAggregatorService(IStudioEventAggregator studioEventAggregator)
		{
			_studioEventAggregator = studioEventAggregator;
		}

		public IDisposable Subscribe<T>(Action<T> action)
		{
			return _studioEventAggregator?.GetEvent<T>().Subscribe(action);
		}

		public void PublishEvent<TEvent>(TEvent sampleEvent)
		{
			_studioEventAggregator?.Publish(sampleEvent);
		}
	}
}
