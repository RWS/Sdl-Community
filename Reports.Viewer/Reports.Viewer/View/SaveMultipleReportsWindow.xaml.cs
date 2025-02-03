using Reports.Viewer.Plus.ViewModel;
using System;
using System.Windows;

namespace Reports.Viewer.Plus.View
{
    public partial class SaveMultipleReportsWindow : Window
    {
        private readonly SaveMultipleReportsViewModel _model;

        public SaveMultipleReportsWindow(SaveMultipleReportsViewModel model)
        {
            _model = model;
            InitializeComponent();
            Loaded += SaveMultipleReports_Loaded;
        }

        private void SaveMultipleReports_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SaveMultipleReports_Loaded;
            DataContext = _model;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
