using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
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
            InitializeComponent();
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
