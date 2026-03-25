using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sdl.Community.DeepLMTProvider.Notifications
{
    public class NotificationGroup : IStudioGroupNotification
    {
        public IStudioNotificationCommand Action { get; set; }
        public bool IsActionVisible { get; set; }
        public string Key { get; set; }
        public ObservableCollection<IStudioNotification> Notifications { get; set; }
        public string Title { get; set; }
    }
}