using Microsoft.Win32;
using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.DeepLMTProvider.Notifications.UI
{
    /// <summary>
    /// Interaction logic for ErrorsWindow.xaml
    /// </summary>
    public partial class ErrorsWindow : Window
    {
        public ErrorsWindow(List<ErrorItem> errorMessages)
        {
            ErrorMessages = errorMessages;
            InitializeComponent();
        }

        public List<ErrorItem> ErrorMessages { get; set; }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not System.Windows.Controls.DataGrid { SelectedItem: ErrorItem selectedError }) return;
            ShowError(selectedError);
        }

        public static void ShowError(ErrorItem selectedError)
        {
            var detailsMessage = $"{selectedError.Message ?? "N/A"}";

            var caption = $"Segment {selectedError.Id} errors";
                

            var errorDetailsWindow = new ErrorDetailsWindow(caption, detailsMessage);
            errorDetailsWindow.ShowDialog();
        }

        private void ExportErrors_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"Translation_Errors_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ExportErrorsToFile(saveFileDialog.FileName);
                    MessageBox.Show($"Errors exported successfully to:\n{saveFileDialog.FileName}",
                                    "Export Complete",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Failed to export errors:\n{ex.Message}",
                                    "Export Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        private void ExportErrorsToFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var content = new StringBuilder();

            if (extension == ".csv")
            {
                // CSV format
                content.AppendLine("Segment ID,Error Message");
                foreach (var error in ErrorMessages)
                {
                    var escapedMessage = error.Message?.Replace("\"", "\"\"") ?? string.Empty;
                    content.AppendLine($"\"{error.Id}\",\"{escapedMessage}\"");
                }
            }
            else
            {
                // Plain text format
                content.AppendLine("Translation Errors Report");
                content.AppendLine($"Generated: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                content.AppendLine($"Total Errors: {ErrorMessages?.Count ?? 0}");
                content.AppendLine(new string('=', 50));
                content.AppendLine();

                if (ErrorMessages?.Any() == true)
                {
                    foreach (var error in ErrorMessages)
                    {
                        content.AppendLine($"Segment ID: {error.Id}");
                        content.AppendLine($"Message: {error.Message}");
                        content.AppendLine(new string('-', 30));
                    }
                }
                else
                {
                    content.AppendLine("No errors to display.");
                }
            }

            File.WriteAllText(filePath, content.ToString(), Encoding.UTF8);
        }
    }
}