using MahApps.Metro.Controls;
using Sdl.Community.StarTransit.UI.Controls;
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
using HockeyApp;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Utils;
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
       
        private PackageModel _package;
        public StarTransitMainWindow(PackageModel package)
        {
            
            InitializeComponent();

            _package = package;
            var packageDetailsViewModel = new PackageDetailsViewModel(package,this);
            _packageDetails = new PackageDetails(packageDetailsViewModel);

            var tmViewModel = new TranslationMemoriesViewModel(packageDetailsViewModel);
            _translationMemories = new TranslationMemories(tmViewModel);
        
            finishViewModel = new FinishViewModel(packageDetailsViewModel);
            _finish = new Finish(finishViewModel);
            
            var starTransitViewModel = new StarTransitMainWindowViewModel(packageDetailsViewModel
                , _packageDetails
                , _translationMemories
                , finishViewModel);
            DataContext = starTransitViewModel;
            if (starTransitViewModel.CloseAction == null)
            {
                starTransitViewModel.CloseAction = Close;
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

      
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
             TelemetryService.Instance.SendCrashes(false);
        }


    }
}
