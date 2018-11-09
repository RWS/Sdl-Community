using System.Windows.Forms;
using Sdl.Community.YourProductivity.Services;

namespace Sdl.Community.YourProductivity.UI
{
    public partial class ProductivityForm : Form
    {
        public ProductivityForm(ProductivityService productivityService)
        {
            InitializeComponent();
            productivityControl.Initialize(productivityService);
        }

    }
}
