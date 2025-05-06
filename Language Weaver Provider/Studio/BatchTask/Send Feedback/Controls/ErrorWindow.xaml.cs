using System.Collections.Generic;
using System.Windows;

namespace LanguageWeaverProvider.Studio.BatchTask.Send_Feedback.Controls
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow(List<FileErrors> errors)
        {
            ErrorList = errors;
            InitializeComponent();
        }

        public List<FileErrors> ErrorList { get; set; }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}