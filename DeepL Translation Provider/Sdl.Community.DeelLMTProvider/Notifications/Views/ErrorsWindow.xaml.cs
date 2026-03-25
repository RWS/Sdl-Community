using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Windows;

namespace Sdl.Community.DeepLMTProvider.Notifications.Views
{
    /// <summary>
    /// Interaction logic for Errors.xaml
    /// </summary>
    public partial class ErrorsWindow : Window
    {
        public ErrorsWindow(List<ErrorItem> errorMessages)
        {
            ErrorMessages = errorMessages;
            InitializeComponent();
        }

        public List<ErrorItem> ErrorMessages { get; set; }
    }
}