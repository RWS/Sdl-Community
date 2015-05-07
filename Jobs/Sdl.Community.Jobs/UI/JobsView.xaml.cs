using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sdl.Community.Jobs.Model;

namespace Sdl.Community.Jobs.UI
{
    /// <summary>
    /// Interaction logic for JobsView.xaml
    /// </summary>
    public partial class JobsView : UserControl
    {

       

        public JobsView()
        {
            EnsureApplicationResources();
            InitializeComponent();
        }

        private void EnsureApplicationResources()
        {
            if (Application.Current == null)
            {
                new Application();

                var controlsResources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml")
                };
                var fontsResources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml")
                };
                var colorsResources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml")
                };
                var blueResources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml")
                };
                var baseLightResources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml")
                };
              
                Application.Current.Resources.MergedDictionaries.Add(controlsResources);
                Application.Current.Resources.MergedDictionaries.Add(fontsResources);
                Application.Current.Resources.MergedDictionaries.Add(colorsResources);
                Application.Current.Resources.MergedDictionaries.Add(blueResources);
                Application.Current.Resources.MergedDictionaries.Add(baseLightResources);

            }
        }

        public void SetJobs(List<JobViewModel> jobs)
        {
            DataGrid.ItemsSource = jobs;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
            e.Handled = true;
        }
    }
}
