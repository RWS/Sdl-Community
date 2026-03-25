using Sdl.Desktop.IntegrationApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sdl.Community.DeepLMTProvider.Notifications
{
    public class NotificationCommand(Action action) : IStudioNotificationCommand
    {
        public event EventHandler CanExecuteChanged;

        public Icon CommandIcon { get; set; }
        public string CommandText { get; set; }
        public string CommandToolTip { get; set; }
        public List<string> ErrorMessages { get; set; }
        private Action Action { get; } = action;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => Action();
    }
}