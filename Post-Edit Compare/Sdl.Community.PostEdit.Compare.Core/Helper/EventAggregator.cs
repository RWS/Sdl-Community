using Sdl.Community.PostEdit.Compare.Core;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.PostEdit.Compare.Core.Helper;

public static class EventAggregator
{
    private static IStudioEventAggregator StudioEventAggregator => SdlTradosStudio.Application.GetService<IStudioEventAggregator>();

    public static void PublishEvent<T>(T message)
    {
        StudioEventAggregator.Publish(message);
    }

    public static void Subscribe<T>(Action<T> action)
    {
        StudioEventAggregator.GetEvent<T>().Subscribe(action);
    }
}