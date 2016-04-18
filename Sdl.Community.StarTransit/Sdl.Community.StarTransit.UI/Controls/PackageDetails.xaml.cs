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
     
        public PackageDetails(PackageModel package)
        {

            InitializeComponent();
            DataContext = new PackageDetailsViewModel(package);
        
            
        }

        //private void GetCultureList()
        //{

        //    var languageList = CultureInfo
        //        .GetCultures(CultureTypes.AllCultures).OrderBy(culture => culture.Name)
        //        .ToList();
        //    //   var languageList = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.DisplayName);
         
        //}

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
        

    }
}
