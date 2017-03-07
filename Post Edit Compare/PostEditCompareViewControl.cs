using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Versions
{
    public partial class PostEditCompareViewControl : UserControl
    {
        public PostEditCompareViewControl()
        {
            InitializeComponent();
        }

        private PostEditCompareViewController _controller { get; set; }
        public PostEditCompareViewController Controller
        {
            get
            {                     
                return _controller;
            }
            set
            {
                _controller = value;
            }
        }

        private void listView_postEditCompareProjectVersions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            label_TOTAL_PROJECT_VERSIONS_SELECTED.Text = listView_postEditCompareProjectVersions.SelectedIndices.Count.ToString();
        }



      
    }
}
