using System;
using System.Windows.Forms;
using Sdl.Community.Jobs.Services;

namespace Sdl.Community.Jobs.UI
{
    public partial class JobsControl : UserControl
    {
        private readonly JobService _service;
        public JobsControl()
        {
            InitializeComponent();
        }

        public JobsControl(JobService service):this()
        {
            _service = service;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _service.SearchCompleted += _service_SearchCompleted;
            _service.Search();
        }


        void _service_SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            ((JobsView)elementHost1.Child).SetJobs(e.JobSearchResults);
        }
    }
}
