using MahApps.Metro.Controls;
using Sdl.Community.StarTransit.UI.Controls;
using System;
using System.Collections.Generic;
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

using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.ViewModels;

namespace Sdl.Community.StarTransit.UI
{
    /// <summary>
    /// Interaction logic for StarTransitMainWindow.xaml
    /// </summary>
    public partial class StarTransitMainWindow : MetroWindow
    {
        private readonly PackageDetails _packageDetails;
        private readonly TranslationMemories _translationMemories;
        private readonly FinishViewModel finishViewModel;
        private readonly Finish _finish;
        public StarTransitMainWindow(PackageModel package)
        {
            EnsureApplicationResources();
            InitializeComponent();
           
          
            var packageDetailsViewModel = new PackageDetailsViewModel(package);
            _packageDetails = new PackageDetails(packageDetailsViewModel);

            _translationMemories = new TranslationMemories();

            finishViewModel = new FinishViewModel(packageDetailsViewModel);
            _finish = new Finish(finishViewModel);
            

             DataContext = new StarTransitMainWindowViewModel(packageDetailsViewModel, _packageDetails,_translationMemories, finishViewModel);
          

        }

        private void EnsureApplicationResources()
        {

            if (Application.Current == null)
            {
                new Application {ShutdownMode = ShutdownMode.OnExplicitShutdown};

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
                var flatButtonsResources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml")
                };

                Application.Current.Resources.MergedDictionaries.Add(controlsResources);
                Application.Current.Resources.MergedDictionaries.Add(fontsResources);
                Application.Current.Resources.MergedDictionaries.Add(colorsResources);
                Application.Current.Resources.MergedDictionaries.Add(blueResources);
                Application.Current.Resources.MergedDictionaries.Add(baseLightResources);
                Application.Current.Resources.MergedDictionaries.Add(flatButtonsResources);
            }
        }


        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            string tag = string.Empty;

            if (sender == null) return;
            var li = sender as ListViewItem;
            if (li == null) return;


            switch (li.Tag.ToString())
            {
                case "packageDetails":

                    tcc.Content = _packageDetails;

                    break;

                case "tm":

                    tcc.Content = _translationMemories;

                    break;
                case "finish":

                    tcc.Content = _finish;


                    break;
                default:
                    tcc.Content = _packageDetails;

                    break;
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            packageDetailsItem.IsSelected = true;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listView_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        
    }
}
