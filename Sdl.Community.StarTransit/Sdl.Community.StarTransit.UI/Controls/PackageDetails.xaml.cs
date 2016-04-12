using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sdl.Community.StarTransit.Shared.Models;
using Forms = System.Windows.Forms;

namespace Sdl.Community.StarTransit.UI.Controls
{
    /// <summary>
    /// Interaction logic for PackageDetails.xaml
    /// </summary>
    public partial class PackageDetails : UserControl
    {
        private PackageModel _package;//= new PackageModel();
      //  private PackageService _packageService;
        public PackageDetails(PackageModel package)
        {

            InitializeComponent();
            txtName.Text = package.Name;
            txtDescription.Text = package.Description;
            comboBox.ItemsSource = package.ProjectTemplate;
            sourceLanguageComboBox.SelectedItem = package.SourceLanguage;
            targetLanguageComboBox.SelectedItem = package.TargetLanguage;

            GetCultureList();
            _package = new PackageModel
            {
                Name = package.Name,
                SourceLanguage = package.SourceLanguage,
                TargetLanguage = package.TargetLanguage,
                ProjectTemplate = package.ProjectTemplate,
                Description = package.Description,
                Files=package.Files
            };
        }

        private void GetCultureList()
        {

            var languageList = CultureInfo
                .GetCultures(CultureTypes.AllCultures).OrderBy(culture => culture.Name)
                .ToList();
            //   var languageList = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.DisplayName);
            sourceLanguageComboBox.ItemsSource = languageList;
            
            targetLanguageComboBox.ItemsSource = languageList;
        }

       

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Forms.FolderBrowserDialog folderDialog = new Forms.FolderBrowserDialog();
            Forms.DialogResult result = folderDialog.ShowDialog();
            
            if(result == Forms.DialogResult.OK)
            {
                txtLocation.Text = folderDialog.SelectedPath;
                AddLocation(folderDialog.SelectedPath);
            }

        }

        private void AddLocation(string selectedPath)
        {
            _package.Location = selectedPath;
            AddLocationToPackage();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            dueDatePicker.IsEnabled = false;

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            dueDatePicker.IsEnabled = true;
        }

        public PackageModel AddLocationToPackage()
        {
            return _package;

        }
    }
}
