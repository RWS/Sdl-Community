using System;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Panels.Main
{
    public partial class QualitivityNavigationControl : UserControl
    {
        public QualitivityNavigationControl()
        {
            InitializeComponent();

            comboBox_filter_name.SelectedIndex = 0;

            treeView_navigation.MouseDown += (sender, args) =>
                treeView_navigation.SelectedNode = treeView_navigation.GetNodeAt(args.X, args.Y);
            
        }

        QualitivityViewController _controller { get; set; }
        public QualitivityViewController Controller
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

        private void button_auto_expand_treeview_MouseHover(object sender, EventArgs e)
        {
            var toolTip1 = new ToolTip();
            toolTip1.SetToolTip(button_auto_expand_treeview, PluginResources.Expand_all_companyProfiles);
        }

        private void button_project_search_MouseHover(object sender, EventArgs e)
        {
            var toolTip1 = new ToolTip();
            toolTip1.SetToolTip(button_project_search, PluginResources.Search_project_name);
        }

       
        private void comboBox_filter_name_KeyPress(object sender, KeyPressEventArgs e)
        {
          
                e.Handled = true;
        }
      
    }
}
