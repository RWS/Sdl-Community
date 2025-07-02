using System;
using System.Windows.Forms;

namespace MySampleTerminology
{
    public partial class TermProviderControl : UserControl
    {
        private WebBrowser webBrowser;

        public TermProviderControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Initialize the WebBrowser control
            webBrowser = new WebBrowser();

            // Configure the WebBrowser properties
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.MinimumSize = new System.Drawing.Size(20, 20);

            // Add the WebBrowser to the UserControl
            Controls.Add(webBrowser);

            // Set the size of the UserControl
            Size = new System.Drawing.Size(800, 600);
        }

        /// <summary>
        /// Loads HTML content into the WebBrowser control.
        /// </summary>
        /// <param name="htmlContent">The HTML string to display.</param>
        public void LoadHtmlContent(string htmlContent)
        {
            webBrowser.DocumentText = htmlContent;
        }

        /// <summary>
        /// Navigates to the specified file path or URL in the WebBrowser control.
        /// </summary>
        /// <param name="filePathOrUrl">The file path or URL to navigate to.</param>
        public void SetNavigation(string filePathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(filePathOrUrl))
            {
                throw new ArgumentException("The file path or URL cannot be null or empty.", nameof(filePathOrUrl));
            }

            try
            {
                this.webBrowser.Navigate(filePathOrUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to navigate to {filePathOrUrl}. Error: {ex.Message}", "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}