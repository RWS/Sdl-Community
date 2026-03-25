using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Sdl.Community.DeepLMTProvider.Notifications.UI
{
    /// <summary>
    /// Interaction logic for ErrorDetailsWindow.xaml
    /// </summary>
    public partial class ErrorDetailsWindow : Window
    {
        public ErrorDetailsWindow(string title, string errorMessage)
        {
            InitializeComponent();
            DataContext = new ErrorDetailsViewModel(title, errorMessage);
            LoadSystemErrorIcon();
        }

        private void LoadSystemErrorIcon()
        {
            try
            {
                // Get the system error icon
                var systemIcon = SystemIcons.Error;
                var iconBitmap = systemIcon.ToBitmap();

                // Convert to WPF ImageSource
                var hBitmap = iconBitmap.GetHbitmap();
                var imageSource = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    System.IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                // Set the image source
                ErrorIcon.Source = imageSource;

                // Clean up
                iconBitmap.Dispose();
                DeleteObject(hBitmap);
            }
            catch
            {
                // If we can't load the system icon, the Image will just remain empty
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(System.IntPtr hObject);

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ErrorDetailsViewModel;
            if (string.IsNullOrEmpty(viewModel?.ErrorMessage)) return;

            try
            {
                Clipboard.SetText(viewModel.ErrorMessage);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Copy Failed", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }

    public class ErrorDetailsViewModel
    {
        public ErrorDetailsViewModel(string title, string errorMessage)
        {
            Title = title;
            ErrorMessage = errorMessage;
        }

        public string Title { get; set; }
        public string ErrorMessage { get; set; }
    }
}