using System;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IEventAggregatorService
	{
		IDisposable Subscribe<T>(Action<T> action);
		void PublishEvent<TEvent>(TEvent sampleEvent);
	}
}
