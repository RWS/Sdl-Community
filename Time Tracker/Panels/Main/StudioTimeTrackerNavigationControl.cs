using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Panels.Main
{
    public partial class StudioTimeTrackerNavigationControl : UserControl
    {
        public StudioTimeTrackerNavigationControl()
        {
            InitializeComponent();

            treeView_navigation.MouseDown += (sender, args) =>
                treeView_navigation.SelectedNode = treeView_navigation.GetNodeAt(args.X, args.Y);
            
        }

        private StudioTimeTrackerViewController _controller { get; set; }
        public StudioTimeTrackerViewController Controller
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

        private void button_auto_expand_treeview_MouseHover(object sender, System.EventArgs e)
        {
            var toolTip = new ToolTip();
            toolTip.SetToolTip(button_auto_expand_treeview, "Expand all clients");
        }

        private void button_project_search_MouseHover(object sender, System.EventArgs e)
        {
            var toolTip = new ToolTip();
            toolTip.SetToolTip(button_project_search, "Search project name");
        }


      

       

        



      
    }
}
