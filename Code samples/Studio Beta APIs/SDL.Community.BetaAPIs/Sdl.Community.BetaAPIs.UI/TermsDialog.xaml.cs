using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sdl.Community.BetaAPIs.UI
{
    /// <summary>
    /// Interaction logic for TermsDialog.xaml
    /// </summary>
    public partial class TermsDialog : Window
    {
        public TermsDialog()
        {
            InitializeComponent();

            LoadTerms();
        }

        private void LoadTerms()
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            var assembly = typeof(TermsDialog).Assembly;
            var resourceName = "Sdl.Community.BetaAPIs.UI.Resources.SDL OpenExchange Terms and Conditions.rtf";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
           // using (var streamReader = new StreamReader(stream))
            {
                textRange.Load(stream, DataFormats.Rtf);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
