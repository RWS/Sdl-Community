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
using Sdl.Community.StarTransit.UI.ViewModels;
using Sdl.ProjectAutomation.Core;
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
            this.DataContext = new PackageDetailsViewModel(package);
            //txtName.Text = package.Name;
            //txtDescription.Text = package.Description;
            //comboBox.ItemsSource = package.StudioTemplates;
            //sourceLanguageBlock.Text = package.SourceLanguage.DisplayName;
            //var targetLanguage = string.Empty;
            //foreach (var language in package.TargetLanguage)
            //{
            //    targetLanguage = targetLanguage + language.DisplayName ;
            //}

            //targetLanguageBlock.Text = targetLanguage;
            //GetCultureList();
            _package = new PackageModel
            {
                Name = package.Name,
                SourceLanguage = package.SourceLanguage,
                TargetLanguage = package.TargetLanguage,
                Description = package.Description,
                SourceFiles = package.SourceFiles,
                TargetFiles = package.TargetFiles
            };

        }

        private void GetCultureList()
        {

            var languageList = CultureInfo
                .GetCultures(CultureTypes.AllCultures).OrderBy(culture => culture.Name)
                .ToList();
            //   var languageList = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.DisplayName);
         
        }

        public  bool FieldsAreCompleted()
        {
            var completed = true;

            if (txtLocation.Text == string.Empty)
            {
                completed= false;
                
            }
            if (comboBox.SelectedItem == null)
            {
                completed = false;
            }
            return completed;
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var templateBox = sender as ComboBox;

            var template = templateBox.SelectedItem as ProjectTemplateInfo;
            _package.ProjectTemplate = template;
        }
    }
}
