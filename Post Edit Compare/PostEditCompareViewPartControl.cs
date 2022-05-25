using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.PostEdit.Versions
{
    public partial class PostEditCompareViewPartControl : UserControl, IUIControl
	{
        
        public PostEditCompareViewPartControl()
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
