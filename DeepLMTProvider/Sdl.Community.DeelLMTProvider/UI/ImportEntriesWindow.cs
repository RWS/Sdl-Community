using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Sdl.Community.DeepLMTProvider.UI
{
    /// <summary>
    /// Interaction logic for ChooseDelimiterWindow.xaml
    /// </summary>
    public partial class ImportEntriesWindow
    {

        public ImportEntriesWindow(List<GlossaryDelimiterItem> glossaryDelimiters)
        {
            Glossaries = glossaryDelimiters;
            InitializeComponent();
        }

        public List<GlossaryDelimiterItem> Glossaries { get; set; }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}