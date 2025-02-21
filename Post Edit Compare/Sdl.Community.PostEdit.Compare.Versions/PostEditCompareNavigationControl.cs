using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.PostEdit.Versions
{
    public partial class PostEditCompareNavigationControl : UserControl, IUIControl
	{
        public PostEditCompareNavigationControl()
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


      

       

        



      
    }
}
