using System.Windows.Forms;

namespace Sdl.Community.PostEdit.Versions
{
    public partial class PostEditCompareNavigationControl : UserControl
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
