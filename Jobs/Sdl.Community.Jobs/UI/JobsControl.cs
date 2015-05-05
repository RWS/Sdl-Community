using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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
        }

        void _service_SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            
        }
    }
}
