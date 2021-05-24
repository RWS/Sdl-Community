using System;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IEventAggregatorService
	{
		void Subscribe<T>(Action<T> action);
		void PublishEvent<TEvent>(TEvent sampleEvent);
	}
}
