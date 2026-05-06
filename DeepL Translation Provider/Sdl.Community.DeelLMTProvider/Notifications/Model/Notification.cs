using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Notifications.Model
{
    public class Notification : IStudioNotification
    {
        public IStudioNotificationCommand Action { get; set; }
        public bool AllowsUserToDismiss { get; set; }
        public List<string> AlwaysVisibleDetails { get; set; }
        public IStudioNotificationCommand ClearNotificationAction { get; set; }
        public Guid Id { get; set; }
        public bool IsActionVisible { get; set; }
        public bool IsExpanderVisible { get; set; }
        public bool IsLinkVisible { get; set; }
        public bool IsRead { get; set; }
        public IStudioNotificationCommand LinkAction { get; set; }
        public List<string> OtherDetails { get; set; }
        public Guid Owner { get; set; }
        public string Title { get; set; }
    }
}